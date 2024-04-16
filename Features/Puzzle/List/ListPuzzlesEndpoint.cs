using ConnectionsAPI.Database;
using ConnectionsAPI.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace ConnectionsAPI.Features.Puzzle.List
{
    public class ListPuzzlesEndpoint(ConnectionsContext db, IAppCache cache) : EndpointWithoutRequest<ICollection<PuzzleDTO>>
    {
        private readonly ConnectionsContext _db = db;
        private readonly IAppCache _cache = cache;

        public override void Configure()
        {
            Get("/all.json");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            // get response from cache
            var response = await _cache.GetOrAddAsync("Puzzle:All",
                                                      GetResponseForCache,
                                                      DateTimeOffset.UtcNow.AddMinutes(5));
            // done
            await SendAsync(response, cancellation: ct);
        }

        private async Task<ICollection<PuzzleDTO>> GetResponseForCache()
        {
            // query all, ordered by print date
            var puzzles = await _db.Puzzles
                .Include(x => x.Categories)
                .ThenInclude(x => x.PuzzleCards)
                .AsNoTracking()
                .OrderBy(x => x.PrintDate)
                .ToListAsync();

            // map to dto
            return puzzles.Select(PuzzleDTO.FromEntity).ToList();
        }
    }
}
