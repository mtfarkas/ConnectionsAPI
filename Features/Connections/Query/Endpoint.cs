using ConnectionsAPI.Models.Request;
using ConnectionsAPI.Models.Response;

namespace ConnectionsAPI.Features.Connections.Query;

public class Endpoint : Endpoint<QueryPuzzlesRequest, PagedDataResponse<ConnectionsPuzzleDTO>>
{
  public override void Configure()
  {
    Get("query");
  }

  public override Task HandleAsync(QueryPuzzlesRequest req, CancellationToken ct)
  {
    return base.HandleAsync(req, ct);
  }
}
