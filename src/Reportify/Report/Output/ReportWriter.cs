using MoreLinq.Extensions;
using Reportify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Reportify.Report.Output;

internal class ReportWriter : IReportWriter
{
  public void Write(Report report)
  {
    var relevantReports = report.DailyReports.Where(d => d.Positions.Any()).ToList();
    if (!relevantReports.Any())
    {
      AnsiConsole.MarkupLine("[yellow]No activities found![/]");
      return;
    }

    foreach (var dailyReport in relevantReports)
    {
      CreateHeaderInfos(dailyReport).ForEach(AnsiConsole.Write);
      AnsiConsole.Write(CreateTable(dailyReport));
    }
  }

  private static Table CreateTable(DailyReport dailyReport)
  {
    var table = new Table();

    CreateColumns()
      .ForEach(c => table.AddColumn(c));

    dailyReport.Positions
      .GroupBy(p => p.ErpPositionId)
      .OrderByDescending(g => g.Sum(p => p.Duration))
      .Select(g => CreateRow(g.Key, g.ToList()))
      .ForEach(r => table.AddRow(r).AddEmptyRow());

    table.RemoveRow(table.Rows.Count - 1);
    return table;
  }

  //TODO: extract calculation sums to dedicated component
  private static IEnumerable<IRenderable> CreateHeaderInfos(DailyReport dailyReport)
  {
    var totalDuration = dailyReport.Positions.Sum(p => p.Duration);
    var roundedTotalDuration = totalDuration.RoundToQuarterHours();
    var sumOfRoundedDurations = dailyReport.Positions
      .GroupBy(p => p.ErpPositionId)
      .Select(g => g.Sum(p => p.Duration))
      .Select(p => p.RoundToQuarterHours())
      .Sum();
    var roundingDeviation = Math.Abs(roundedTotalDuration - sumOfRoundedDurations);

    var ruleMarkup = roundingDeviation > 0
      ? $"Report for {dailyReport.Date} - Total: [darkorange][bold]{sumOfRoundedDurations:F2}h[/][/] ({totalDuration:hh\\:mm})"
      : $"Report for {dailyReport.Date} - Total: [bold]{sumOfRoundedDurations:F2}h[/] [dim]({totalDuration:hh\\:mm})[/]";

    yield return new Padder(new Rule(ruleMarkup).LeftJustified(), new Padding(0, 1, 0, 0));
    if (roundingDeviation == 0) yield break;

    var roundingDeviationInfo =
      $"[yellow]The sum of all rounded durations ({sumOfRoundedDurations:F2}h) differs from the rounded total duration ({roundedTotalDuration:F2}h) by {roundingDeviation:F2}h.[/]";
    var roundingDeviationInstruction =
      "[yellow]Please adjust your report manually so that the total sum adds up again.[/]";

    yield return new Padder(new Markup(roundingDeviationInfo), new Padding(1, 0));
    yield return new Padder(new Markup(roundingDeviationInstruction), new Padding(1, 0));
  }

  private static IEnumerable<TableColumn> CreateColumns()
  {
    yield return new TableColumn("Position");
    yield return new TableColumn("Duration (h)");
    yield return new TableColumn("Activities");
  }

  private static IEnumerable<IRenderable> CreateRow(int? erpPosition, IReadOnlyList<Position> positions)
  {
    yield return new Markup(FormatErpNumber(erpPosition));
    yield return new Markup(FormatDuration(positions.Sum(p => p.Duration)));
    yield return CreatePositionsTable(positions);
  }

  private static IRenderable CreatePositionsTable(IReadOnlyList<Position> positions)
  {
    var table = new Table()
      .AddColumns(string.Empty, string.Empty)
      .Expand()
      .HideHeaders()
      .NoBorder();

    var multipleItems = positions.Count > 1;
    var prefix = multipleItems ? "- " : string.Empty;

    positions
      .OrderByDescending(p => p.Duration)
      .ForEach(
        p => table.AddRow(
          new Markup($"{prefix}{EscapeMarkup(p.Name)}"),
          new Padder(new Text($"{p.Duration:hh\\:mm}").RightJustified(), new Padding(1, 0, 0, 0)))
      );

    return table;
  }

  private static string FormatErpNumber(int? number) => number == null ? "-" : $"[bold]{number:000 000}[/]";

  private static string FormatDuration(TimeSpan duration)
  {
    var roundedDuration = duration.RoundToQuarterHours();
    return $"[bold]{roundedDuration:F2}[/] [dim]({duration:hh\\:mm})[/]";
  }

  private static string EscapeMarkup(string input) => input.Replace("[", "(").Replace("]", ")");
}