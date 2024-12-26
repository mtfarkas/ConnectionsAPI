using ConnectionsAPI.Database.Repository;
using ConnectionsAPI.Models.Request;
using ConnectionsAPI.Models.Response;

namespace ConnectionsAPI.Features.Connections.Get;

public class GetConnectionsEndpoint(PuzzleRepository _puzzleRepo) : Endpoint<GetPuzzleRequest, ConnectionsPuzzleDTO>
{
  public override void Configure()
  {
    Get("{PrintDate}");
    Group<ConnectionsGroup>();
  }

  public override async Task HandleAsync(GetPuzzleRequest req, CancellationToken ct)
  {
    bool hideSolutions = Query<bool>("hideSolutions", isRequired: false);

    // query for the puzzle
    var puzzle = await _puzzleRepo.GetPuzzleByDateAsync(req.PrintDate, includeSolutions: !hideSolutions);

    // if not found, done here
    if (puzzle == null)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    // get response from cache
    var response = ConnectionsPuzzleDTO.FromEntity(puzzle);

    // done
    await SendAsync(response, cancellation: ct);
  }
}