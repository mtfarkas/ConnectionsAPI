using System;
using System.IO.Compression;
using ConnectionsAPI.Models.Request;
using FluentValidation;

namespace ConnectionsAPI.Validators;

public class QueryPuzzlesRequestValidator : Validator<QueryPuzzlesRequest>
{
  public QueryPuzzlesRequestValidator()
  {
    RuleFor(x => x.Page)
      .Must(x => x > 0)
        .WithMessage("Page number must be a positive integer");

    RuleFor(x => x.Count)
      .Must(x => x > 0)
        .WithMessage(x => "Item count must be a positive integer");

    RuleFor(x => x.Year)
      .Must(x => x == null || (x != null && x >= 2021 && x <= DateTime.UtcNow.Year + 1))
        .WithMessage($"Year must be a valid year between 2021 and {DateTime.UtcNow.Year + 1}");

    RuleFor(x => x.Month)
      .Must(x => x == null || (x != null && x >= 1 && x <= 12))
        .WithMessage("Month must be a valid month");
  }
}
