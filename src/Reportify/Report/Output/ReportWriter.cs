using System.Text;
using MoreLinq.Extensions;
using Reportify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Reportify.Report.Output;

internal class ReportWriter : IReportWriter
{
  public void Write(OutputData outputData)
  {
    if (!outputData.DailySummaries.Any())
    {
      AnsiConsole.MarkupLine("[yellow]No activities found![/]");
      return;
    }

    foreach (var dailySummary in outputData.DailySummaries)
    {
      CreateHeaderInfos(dailySummary).ForEach(AnsiConsole.Write);
      AnsiConsole.Write(CreateTable(dailySummary));
    }
  }

  private static Table CreateTable(DailySummary dailySummary)
  {
    var table = new Table()
      .AddColumns(CreateColumns().ToArray());

    dailySummary.PositionSummaries
      .Select(CreateRow)
      .ForEach(r => table.AddRow(r).AddEmptyRow());

    table.RemoveRow(table.Rows.Count - 1);
    return table;
  }

  private static IEnumerable<IRenderable> CreateHeaderInfos(DailySummary dailySummary)
  {
    var roundedTotalDuration = dailySummary.TotalDuration.RoundToQuarterHours();
    var roundingDeviation = Math.Abs(roundedTotalDuration - dailySummary.SumOfRoundedDurationsInHours);

    var headerMarkup = new StringBuilder()
      .Append($"Report for {dailySummary.Date} - Total: ")
      .Append($"[{(roundingDeviation > 0 ? "darkorange" : "bold")}]")
      .Append($"{dailySummary.SumOfRoundedDurationsInHours:F2}h[/] ")
      .Append($"[dim]({dailySummary.TotalDuration:hh\\:mm})[/]")
      .ToString();

    yield return new Padder(new Rule(headerMarkup).LeftJustified(), new Padding(0, 1, 0, 0));

    if (roundingDeviation == 0) yield break;

    var roundingDeviationInfo = new StringBuilder()
      .Append($"[yellow]The sum of all rounded durations ({dailySummary.SumOfRoundedDurationsInHours:F2}h) ")
      .Append($"differs from the rounded total duration ({roundedTotalDuration:F2}h) by {roundingDeviation:F2}h.[/]")
      .AppendLine()
      .Append("[yellow]Please adjust your report manually so that the total sum adds up again.[/]")
      .ToString();

    yield return new Padder(new Markup(roundingDeviationInfo), new Padding(1, 0));
  }

  private static IEnumerable<TableColumn> CreateColumns()
  {
    yield return new TableColumn("Position");
    yield return new TableColumn("Duration (h)");
    yield return new TableColumn("Activities");
  }

  private static IEnumerable<IRenderable> CreateRow(PositionSummary positionSummary)
  {
    yield return new Markup(FormatErpNumber(positionSummary.PositionId));
    yield return new Markup(FormatDuration(positionSummary.Duration, positionSummary.RoundedDurationInHours));
    yield return CreateInnerTable(positionSummary.Activities.ToList());
  }

  private static IRenderable CreateInnerTable(IReadOnlyCollection<Activity> activities)
  {
    var table = new Table()
      .AddColumns(string.Empty, string.Empty)
      .Expand()
      .HideHeaders()
      .NoBorder();

    var multipleItems = activities.Count > 1;
    var prefix = multipleItems ? "- " : string.Empty;

    activities
      .ForEach(
        p => table.AddRow(
          new Markup($"{prefix}{EscapeMarkup(p.Name)}"),
          new Padder(new Text($"{p.Duration:hh\\:mm}").RightJustified(), new Padding(1, 0, 0, 0)))
      );

    return table;
  }

  private static string FormatErpNumber(int? number) => number == null ? "-" : $"[bold]{number:000 000}[/]";

  private static string FormatDuration(TimeSpan duration, double durationInHours) =>
    $"[bold]{durationInHours:F2}[/] [dim]({duration:hh\\:mm})[/]";


  private static string EscapeMarkup(string input) => input.Replace("[", "(").Replace("]", ")");
}