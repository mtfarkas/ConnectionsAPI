using System;
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
  }
}
