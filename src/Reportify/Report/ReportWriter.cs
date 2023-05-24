using MoreLinq.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Reportify.Report;

internal class ReportWriter : IReportWriter
{
  public void Write(Report report)
  {
    var tables = report.DailyReports.Select(CreateTable);

    foreach (var table in tables)
    {
      AnsiConsole.WriteLine();
      AnsiConsole.Write(table);
    }
  }

  private static Table CreateTable(DailyReport dailyReport)
  {
    var table = new Table();
    table.Title(CreateTitle(dailyReport));

    CreateColumns()
      .ForEach(c => table.AddColumn(c));

    dailyReport.Positions
      .GroupBy(p => p.ErpPositionId)
      .OrderByDescending(g => g.Sum(p => p.Duration))
      .Select(g => CreateRow(g.Key, g.ToList()))
      .ForEach(r => table.AddRow(r).AddEmptyRow());

    return table;
  }

  private static TableTitle CreateTitle(DailyReport dailyReport)
  {
    //TODO display no table if no positions exist
    var totalDuration = dailyReport.Positions.Any()
      ? dailyReport.Positions.Sum(p => p.Duration)
      : TimeSpan.Zero;

    return new TableTitle($"Report for {dailyReport.Date}. Total: {totalDuration:hh\\:mm}");
  }

  private static IEnumerable<TableColumn> CreateColumns()
  {
    yield return new TableColumn("Erp-Position");
    yield return new TableColumn("Duration (h)");
    yield return new TableColumn("Activities");
  }

  private static IEnumerable<IRenderable> CreateRow(int? erpPosition, IReadOnlyList<Position> positions)
  {
    yield return new Text(FormatErpNumber(erpPosition));
    yield return new Markup(FormatDuration(positions.Sum(p => p.Duration)));
    yield return CreatePositionsTable(positions);
  }

  private static IRenderable CreatePositionsTable(IEnumerable<Position> positions)
  {
    var table = new Table()
      .AddColumns(string.Empty, string.Empty)
      .Expand()
      .HideHeaders()
      .NoBorder();

    positions
      .OrderByDescending(p => p.Duration)
      .ForEach(
        p => table.AddRow(
          new Text(p.Name),
          new Padder(new Text($"{p.Duration:hh\\:mm}").RightJustified(), new Padding(2, 0, 0, 0)))
      );

    return table;
  }

  private static string FormatErpNumber(int? number) => number == null ? "-" : $"{number:000 000}";

  private static string FormatDuration(TimeSpan duration)
  {
    var roundedHours = Math.Round(duration.TotalHours * 4, MidpointRounding.ToEven) / 4;
    return $"[bold]{roundedHours:F2}[/] [dim]({duration:hh\\:mm})[/]";
  }
}