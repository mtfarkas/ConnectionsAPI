namespace ConnectionsAPI.Models.Request;

public class QueryPuzzlesRequest
{
  [QueryParam] public int Page { get; set; }
  [QueryParam] public int Count { get; set; }

  [QueryParam] public int? Year { get; set; }
  [QueryParam] public int? Month { get; set; }
}
