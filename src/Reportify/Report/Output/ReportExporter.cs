using Spectre.Console;
using TextCopy;

namespace Reportify.Report.Output;

internal class ReportExporter : IReportExporter
{
  public void ExportToClipboard(OutputData outputData)
  {
    var csv = BuildCsvString(outputData.DailySummaries);
    ClipboardService.SetText(csv);

    AnsiConsole.MarkupLine("[green]Report copied to clipboard[/]");
  }

  private static string BuildCsvString(IEnumerable<DailySummary> dailySummaries)
  {
    var positions = dailySummaries
      .SelectMany(d => d.PositionSummaries.Select(p => new { d.Date, p.PositionId, p.RoundedDurationInHours, p.Notes }))
      .Where(position => position.PositionId != null)
      .OrderBy(position => position.Date)
      .ThenByDescending(position => position.RoundedDurationInHours);

    return positions
      .Select(position => FormatPosition(position.Date, position.PositionId, position.RoundedDurationInHours, position.Notes))
      .Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
  }

  private static string FormatPosition(DateOnly date, int? erpPositionId, double duration, string? notes)
  {
    return string.Join(";", date.ToString("dd.MM.yyyy"), erpPositionId, duration.ToString("F2"), notes);
  }
}