// using ConnectionsAPI.Database.Repository;
// using ConnectionsAPI.Models;
// using LazyCache;
// using Microsoft.EntityFrameworkCore;

// namespace ConnectionsAPI.Features.Puzzle.List
// {
//     public class ListPuzzlesEndpoint(PuzzleRepository puzzleRepo, IAppCache cache) : EndpointWithoutRequest<ICollection<ConnectionsPuzzleDTO>>
//     {
//         private readonly PuzzleRepository _puzzleRepo = puzzleRepo;
//         private readonly IAppCache _cache = cache;

//         public override void Configure()
//         {
//             Get("/all.json",
//                 "/puzzle/all");
//             AllowAnonymous();
//         }

//         public override async Task HandleAsync(CancellationToken ct)
//         {
//             bool hideSolutions = Query<bool>("hideSolutions", isRequired: false);

//             // query all, ordered by print date
//             var puzzles = await _puzzleRepo.GetAllPuzzlesAsync(includeSolutions: !hideSolutions);

//             // map to response object
//             var response = puzzles.Select(ConnectionsPuzzleDTO.FromEntity).ToList();

//             // done
//             await SendAsync(response, cancellation: ct);
//         }
//     }
// }
