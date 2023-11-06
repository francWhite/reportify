using System.Globalization;
using Reportify.Extensions;
using Spectre.Console;
using TextCopy;

namespace Reportify.Report.Output;

internal class ReportExporter : IReportExporter
{
  public void ExportToClipboard(Report report)
  {
    var csv = BuildCsvString(report.DailyReports);
    ClipboardService.SetText(csv);

    AnsiConsole.MarkupLine("[green]Report copied to clipboard[/]");
  }

  private static string BuildCsvString(IEnumerable<DailyReport> dailyReports)
  {
    var positions = dailyReports
      .SelectMany(
        report => report.Positions
          .GroupBy(position => position.ErpPositionId)
          .Select(
            group => new
            {
              report.Date,
              ErpPositionId = group.Key,
              Duration = group.Sum(position => position.Duration)
            }))
      .Where(position => position.ErpPositionId != null)
      .OrderBy(position => position.Date)
      .ThenByDescending(position => position.Duration);

    return positions
      .Select(position => FormatPosition(position.Date, position.ErpPositionId, position.Duration))
      .Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
  }

  private static string FormatPosition(DateOnly date, int? erpPositionId, TimeSpan duration)
  {
    FormattableString formattable = $"{date:dd.MM.yyyy};{erpPositionId};{duration.RoundToQuarterHours():F2}";
    return formattable.ToString(CultureInfo.InvariantCulture);
  }
}