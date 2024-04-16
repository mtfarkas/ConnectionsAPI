using ConnectionsAPI.Database;
using ConnectionsAPI.Models;
using FluentValidation;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ConnectionsAPI.Features.Puzzle.Get
{
    public record GetPuzzleEndpointRequest(string PuzzleDate);

    public partial class GetPuzzleEndpointRequestValidator : Validator<GetPuzzleEndpointRequest>
    {
        [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.IgnoreCase)]
        private static partial Regex PrintDateGeneratedRegex();

        public GetPuzzleEndpointRequestValidator()
        {
            RuleFor(x => x.PuzzleDate)
                .NotEmpty().WithMessage("Puzzle date is required")
                .Must(x => PrintDateGeneratedRegex().IsMatch(x)).WithMessage("Puzzle date must be in the format yyyy-MM-dd");
        }
    }

    public class GetPuzzleEndpoint(ConnectionsContext db, ILogger<GetPuzzleEndpoint> logger, IAppCache cache) : Endpoint<GetPuzzleEndpointRequest, PuzzleDTO>
    {
        private readonly ConnectionsContext _db = db;
        private readonly ILogger<GetPuzzleEndpoint> _logger = logger;
        private readonly IAppCache _cache = cache;

        public override void Configure()
        {
            Get("/{PuzzleDate}.json");
            AllowAnonymous();
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(GetPuzzleEndpointRequest req, CancellationToken ct)
        {
            // default to 404 if validation fails
            if (ValidationFailed)
            {
                _logger.LogError("Validation error. {path} {pathBase}", HttpContext.Request.Path, HttpContext.Request.PathBase);
                await SendNotFoundAsync(ct);
                return;
            }

            // get response from cache
            var response = await _cache.GetOrAddAsync($"Puzzle:{req.PuzzleDate}",
                                                      () => { return GetResponseForCache(req.PuzzleDate); },
                                                      DateTimeOffset.UtcNow.AddMinutes(5));

            // if not found, done here
            if (response == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // done
            await SendAsync(response, cancellation: ct);
        }

        private async Task<PuzzleDTO?> GetResponseForCache(string printDate)
        {
            // query for the puzzle
            var puzzle = await _db.Puzzles
                .Include(x => x.Categories)
                .ThenInclude(x => x.PuzzleCards)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PrintDate == printDate);

            // if not found, we're done here
            if (puzzle == null)
            {
                return null;
            }

            // if found, map
            return PuzzleDTO.FromEntity(puzzle);
        }
    }
}
