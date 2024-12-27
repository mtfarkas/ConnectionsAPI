using ConnectionsAPI.Database.Repository;
using ConnectionsAPI.Models.Request;
using ConnectionsAPI.Models.Response;

namespace ConnectionsAPI.Features.Connections.Query;

public class Endpoint(PuzzleRepository _puzzleRepository) : Endpoint<QueryPuzzlesRequest, PagedDataResponse<ConnectionsPuzzleDTO>>
{
  public override void Configure()
  {
    Get("query");
    Group<ConnectionsGroup>();
  }

  public override async Task HandleAsync(QueryPuzzlesRequest req, CancellationToken ct)
  {
    bool hideSolutions = Query<bool>("hideSolutions", isRequired: false);

    var puzzles = await _puzzleRepository.QueryConnectionsPuzzles(req.Page,
                                                                  req.Count,
                                                                  req.Year,
                                                                  req.Month,
                                                                  !hideSolutions);

    PagedDataResponse<ConnectionsPuzzleDTO> response = new(puzzles.Page, puzzles.Count, puzzles.MaxCount, puzzles.Data.Select(ConnectionsPuzzleDTO.FromEntity).ToList());

    await SendAsync(response, cancellation: ct);
  }
}
