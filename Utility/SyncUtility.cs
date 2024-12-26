using ConnectionsAPI.Database;
using ConnectionsAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConnectionsAPI.Utility
{
    public class SyncUtility(ConnectionsContext db, ILogger<SyncUtility> logger, HttpClient http)
    {
        #region Response types
        class NYTConnectionsPuzzle
        {
            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;
            [JsonPropertyName("print_date")]
            public string PrintDate { get; set; } = string.Empty;
            [JsonPropertyName("editor")]
            public string Editor { get; set; } = string.Empty;
            [JsonPropertyName("categories")]
            public IReadOnlyList<NYTConnectionsPuzzleCategory> Categories { get; set; } = [];

            [JsonIgnore]
            public string Md5 { get; set; } = string.Empty;
        }

        class NYTConnectionsPuzzleCategory
        {
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
            [JsonPropertyName("cards")]
            public IReadOnlyList<NYTConnectionsPuzzleCard> Cards { get; set; } = [];
        }

        public class NYTConnectionsPuzzleCard
        {
            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
            [JsonPropertyName("position")]
            public int Position { get; set; }
        }
        #endregion

        private static readonly string SHORT_DATE = "yyyy-MM-dd";

        private readonly ConnectionsContext _db = db;
        private readonly ILogger<SyncUtility> _logger = logger;
        private readonly HttpClient _http = http;

        public async Task SyncPuzzlesAsync(CancellationToken ct)
        {
            _logger.LogInformation("Calculating puzzle sync dates");

            // calculate the date ranges for the sync
            var syncDates = await GetSyncDatesAsync(ct);

            _logger.LogInformation("Syncing puzzles between {start} - {end}", syncDates[0], syncDates[^1]);

            // run the HTTP requests in batches
            ConcurrentDictionary<string, string> responses = new();
            foreach (var batch in syncDates.Chunk(5))
            {
                ConcurrentBag<NYTConnectionsPuzzle> batchPuzzles = [];
                await Task.WhenAll(
                    batch.Select(x => GetConnectionsResponseAsync(x, ct).ContinueWith(t =>
                    {
                        string? result = t.Result;
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            try
                            {
                                var nytResponseJson = JsonSerializer.Deserialize<NYTConnectionsPuzzle>(result)
                                    ?? throw new InvalidDataException("Connections response deserialized to null");

                                string md5 = HashUtility.CalculateMD5(result);
                                nytResponseJson.Md5 = md5;
                                batchPuzzles.Add(nytResponseJson);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to deserialize Connections response for {date}", x);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Puzzle {date} non-success response, skipping", x);
                        }
                    }))
                );

                foreach (var puzzle in batchPuzzles.OrderBy(x => x.PrintDate))
                {
                    await UpsertPuzzleDataAsync(puzzle);
                }
                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task UpsertPuzzleDataAsync(NYTConnectionsPuzzle nytPuzzle)
        {
            // get a tracking reference to the puzzle matching by print date, either by querying or creating a new entity
            var puzzle = await _db.CategoriesPuzzles
                .Include(x => x.Categories)
                .ThenInclude(x => x.CategoriesPuzzleCards)
                .FirstOrDefaultAsync(x => x.PrintDate == nytPuzzle.PrintDate);
            if (puzzle == null)
            {
                _logger.LogTrace("No puzzle found for {printDate}, puzzle will be created", nytPuzzle.PrintDate);
                puzzle = new Database.Entities.CategoriesPuzzle
                {
                    Categories = [],
                    CreatedDate = DateTime.UtcNow
                };
                _db.CategoriesPuzzles.Add(puzzle);
            }

            // if the content hash matches, no update needed
            if (puzzle.ContentMD5 == nytPuzzle.Md5)
            {
                _logger.LogTrace("JSON content hash for {printDate} matches, no need for update", nytPuzzle.PrintDate);
                return;
            }

            puzzle.ContentMD5 = nytPuzzle.Md5;
            puzzle.PrintDate = nytPuzzle.PrintDate;
            puzzle.EditorName = nytPuzzle.Editor;
            puzzle.Index = CalculateConnectionsDayIndex(nytPuzzle.PrintDate);
            puzzle.Categories ??= [];

            // mark items for deletion and also remove them from here to be readded
            _db.RemoveRange(puzzle.Categories);
            puzzle.Categories.Clear();

            // construct the entities
            int idx = 1;
            foreach (var nytCategory in nytPuzzle.Categories)
            {
                CategoriesCategory category = new()
                {
                    Color = (CategoriesColor)idx++,
                    Name = nytCategory.Title,
                    CategoriesPuzzle = puzzle,
                    CategoriesPuzzleCards = []
                };

                foreach (var nytCard in nytCategory.Cards)
                {
                    CategoriesCard card = new()
                    {
                        Content = nytCard.Content,
                        Position = nytCard.Position,
                        Category = category,
                    };
                    category.CategoriesPuzzleCards.Add(card);
                }

                puzzle.Categories.Add(category);
            }
            // done
        }

        private static int CalculateConnectionsDayIndex(string printDate)
        {
            DateTime connectionsDate = DateTime.ParseExact(printDate, SHORT_DATE, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;
            return Convert.ToInt32(Math.Max((connectionsDate - Constants.ConnectionsStartDate).TotalDays, 0)) + 1;
        }

        private async Task<string?> GetConnectionsResponseAsync(string printDate, CancellationToken ct)
        {
            string url = $"https://www.nytimes.com/svc/connections/v2/{printDate}.json";
            using var resp = await _http.GetAsync(url, ct);
            if (resp == null || !resp.IsSuccessStatusCode)
            {
                return null;
            }
            string responseContent = await resp.Content.ReadAsStringAsync(ct);
            return responseContent;
        }

        private async Task<IReadOnlyList<string>> GetSyncDatesAsync(CancellationToken ct)
        {
            // query the last puzzle we have in the database
            string? lastSyncedPuzzleDate = await _db.CategoriesPuzzles.AsNoTracking()
                .OrderByDescending(x => x.PrintDate)
                .Select(x => x.PrintDate)
                .FirstOrDefaultAsync(cancellationToken: ct);

            // calculate the starting date of the sync
            string startDate;

            // if no puzzle was synced before, we use the start day of connections as a start (we want to sync every puzzle ever)
            if (string.IsNullOrWhiteSpace(lastSyncedPuzzleDate))
            {
                startDate = Constants.ConnectionsStartDate.ToString(SHORT_DATE);
            }
            else
            {
                string todayPrintDate = DateTimeOffset.UtcNow.UtcDateTime.ToString(SHORT_DATE);
                // if we have a puzzle, we check the latest print date we have
                // if the print date is earlier than today's date, we use that day as a base
                // if the print date is after today, we use today as a base
                if (lastSyncedPuzzleDate.CompareTo(todayPrintDate) < 0)
                {
                    startDate = lastSyncedPuzzleDate;
                }
                else
                {
                    startDate = todayPrintDate;
                }
            }

            // construct a list of dates
            List<string> dates = [startDate];

            // we iterate on every day between the start date and UTC tomorrow (this should handle +12 timezones as well)
            DateTime syncBeginDate = DateTime.ParseExact(startDate, SHORT_DATE, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;
            DateTime syncEndDate;

            // try to find the latest date that is currently going on in the world
            TimeZoneInfo? latestTimezone = TimezoneUtility.GetLatestTimezoneOnSystem();
            if (latestTimezone != null)
            {
                DateTime currentDateInLatestTimezone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, latestTimezone);
                syncEndDate = new DateTime(currentDateInLatestTimezone.Year, currentDateInLatestTimezone.Month, currentDateInLatestTimezone.Day, 0, 0, 0, DateTimeKind.Utc);
            }
            // default to UTC date + 1 day
            else
            {
                syncEndDate = DateTime.UtcNow.Date.AddDays(1);
            }

            foreach (var date in Enumerable.Repeat(0, Convert.ToInt32((syncEndDate - syncBeginDate).TotalDays)).Select((_, idx) => syncBeginDate.AddDays(idx + 1)))
            {
                dates.Add(date.ToString(SHORT_DATE));
            }

            // done
            return dates;
        }
    }
}
