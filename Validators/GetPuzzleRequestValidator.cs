using System.Globalization;
using System.Text.RegularExpressions;
using ConnectionsAPI.Models.Request;
using FluentValidation;

namespace ConnectionsAPI.Validators;

public partial class GetPuzzleRequestValidator : Validator<GetPuzzleRequest>
{
  [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.IgnoreCase)]
  private static partial Regex PrintDateGeneratedRegex();

  public GetPuzzleRequestValidator()
  {
    RuleFor(x => x.PrintDate)
      .NotEmpty().WithMessage("Puzzle date is required")
      .Must(x => PrintDateGeneratedRegex().IsMatch(x))
        .WithMessage("Print date must be in the format yyyy-MM-dd")
      .Must(x => DateTime.TryParseExact(x, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _))
        .WithMessage("Print date must be a valid datetime");

  }
}
