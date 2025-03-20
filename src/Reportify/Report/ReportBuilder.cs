using System.Text.RegularExpressions;
using Reportify.Report.ManicTime;

namespace Reportify.Report;

internal partial class ReportBuilder : IReportBuilder
{
  private readonly IActivityQuery _activityQuery;
  private readonly IErpPositionEvaluator _erpPositionEvaluator;

  public ReportBuilder(IActivityQuery activityQuery, IErpPositionEvaluator erpPositionEvaluator)
  {
    _activityQuery = activityQuery;
    _erpPositionEvaluator = erpPositionEvaluator;
  }

  public async Task<Report> BuildAsync(DateOnly date, CancellationToken cancellationToken = default)
  {
    var dailyReport = await BuildDailyReportAsync(date, cancellationToken);
    return new Report(new[] { dailyReport });
  }

  public async Task<Report> BuildAsync(DateOnly startDate, DateOnly endDate,
    CancellationToken cancellationToken = default)
  {
    var dailyReportTasks = Enumerable
      .Range(0, endDate.DayNumber - startDate.DayNumber + 1)
      .Select(startDate.AddDays)
      .Select(date => BuildDailyReportAsync(date, cancellationToken));

    var dailyReports = await Task.WhenAll(dailyReportTasks);
    return new Report(dailyReports);
  }

  private async Task<DailyReport> BuildDailyReportAsync(DateOnly date, CancellationToken cancellationToken)
  {
    var activities = await _activityQuery.GetAsync(date, cancellationToken);
    var positionTasks = activities.Select(
      async a => new Position(
        a.Name,
        TimeSpan.FromSeconds(a.TotalSeconds),
        await _erpPositionEvaluator.EvaluateAsync(a.Name, cancellationToken),
        GetNote(a.Name)));

    var positions = await Task.WhenAll(positionTasks);
    return new DailyReport(date, positions);
  }

  private static string? GetNote(string name)
  {
    var notesMatch = NotesRegex().Match(name);
    return notesMatch.Success ? notesMatch.Groups["notes"].Value : null;
  }

  [GeneratedRegex("%(?<notes>.*)")]
  private static partial Regex NotesRegex();
}