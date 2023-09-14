using Spectre.Console;

namespace Reportify.Configuration.Validation;

internal class ValidationErrorWriter : IValidationErrorWriter
{
  public void Write(IReadOnlyList<ValidationError> validationErrors)
  {
    if (!validationErrors.Any())
      return;

    var errorMessages = validationErrors
      .Select(v => new Padder(new Text($"- {v.Message}"), new Padding(2, 0)))
      .ToList();

    AnsiConsole.MarkupLine(
      $"[red]Configuration file [underline]{ConfigurationFileInfo.FilePath}[/] is invalid, please fix the following errors.[/]");
    AnsiConsole.Write(new Rows(errorMessages));
  }
}