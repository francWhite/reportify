using MoreLinq.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Reportify.Report;

internal class ReportWriter : IReportWriter
{
  public void Write(Report report)
  {
    var relevantReports = report.DailyReports.Where(d => d.Positions.Any()).ToList();
    if (!relevantReports.Any())
    {
      AnsiConsole.MarkupLine("[darkgoldenrod]No activities found![/]");
      return;
    }

    foreach (var dailyReport in relevantReports)
    {
      AnsiConsole.Write(CreateTitle(dailyReport));
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

  private static IRenderable CreateTitle(DailyReport dailyReport)
  {
    var totalDuration = dailyReport.Positions.Sum(p => p.Duration);
    return new Padder(
      new Rule($"Report for {dailyReport.Date}. Total: {totalDuration:hh\\:mm}h").LeftJustified(),
      new Padding(0, 1, 0, 0));
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
    var roundedHours = Math.Round(duration.TotalHours * 4, MidpointRounding.ToEven) / 4;
    return $"[bold]{roundedHours:F2}[/] [dim]({duration:hh\\:mm})[/]";
  }

  private static string EscapeMarkup(string input) => input.Replace("[", "(").Replace("]", ")");

}