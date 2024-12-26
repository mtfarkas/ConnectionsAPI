namespace ConnectionsAPI.Models.Request;

public class QueryPuzzlesRequest
{
  [QueryParam] public int Page { get; set; }
  [QueryParam] public int Count { get; set; }
}
