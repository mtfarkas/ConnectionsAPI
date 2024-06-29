using ConnectionsAPI.Database.Repository;
using ConnectionsAPI.Models;
using FluentValidation;
using LazyCache;
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

    public class GetPuzzleEndpoint(PuzzleRepository puzzleRepo, ILogger<GetPuzzleEndpoint> logger, IAppCache cache) : Endpoint<GetPuzzleEndpointRequest, PuzzleDTO>
    {
        private readonly PuzzleRepository _puzzleRepo = puzzleRepo;
        private readonly ILogger<GetPuzzleEndpoint> _logger = logger;
        private readonly IAppCache _cache = cache;

        public override void Configure()
        {
            Get("/{PuzzleDate}.json",
                "/puzzle/{PuzzleDate}");
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

            bool hideSolutions = Query<bool>("hideSolutions", isRequired: false);

            // query for the puzzle
            var puzzle = await _puzzleRepo.GetPuzzleByDateAsync(req.PuzzleDate, includeSolutions: !hideSolutions);

            // if not found, done here
            if (puzzle == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // get response from cache
            var response = PuzzleDTO.FromEntity(puzzle);

            // done
            await SendAsync(response, cancellation: ct);
        }
    }
}
