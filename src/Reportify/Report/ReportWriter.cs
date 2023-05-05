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
      .Select(CreateRow)
      .ForEach(r => table.AddRow(r));

    return table;
  }

  private static TableTitle CreateTitle(DailyReport dailyReport)
  {
    var totalDuration = dailyReport.Positions
      .Select(p => p.Duration)
      .Aggregate((total, current) => total.Add(current));

    return new TableTitle($"Report for {dailyReport.Date}. Total: {totalDuration:hh\\:mm}");
  }

  private static IEnumerable<TableColumn> CreateColumns()
  {
    yield return new TableColumn("Activity");
    yield return new TableColumn("Erp-Contract");
    yield return new TableColumn("Erp-Position");
    yield return new TableColumn("Duration (h)");
  }

  private static IEnumerable<IRenderable> CreateRow(Position position)
  {
    yield return new Text(position.Name);
    yield return new Text(FormatErpNumber(position.ErpContract));
    yield return new Text(FormatErpNumber(position.ErpPosition));
    yield return new Markup(FormatDuration(position.Duration));
  }

  private static string FormatErpNumber(int? number) => number == null ? string.Empty : $"{number:000 000}";

  private static string FormatDuration(TimeSpan duration)
  {
    var roundedHours = Math.Round(duration.TotalHours * 4, MidpointRounding.ToEven) / 4;
    return $"[bold]{roundedHours:F2}[/] [dim]({duration:hh\\:mm})[/]";
  }
}