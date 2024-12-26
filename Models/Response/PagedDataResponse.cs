namespace ConnectionsAPI.Models.Response;

public record PagedDataResponse<T>(
  int Page,
  int Count,
  int MaxCount,
  ICollection<T> Data
);
