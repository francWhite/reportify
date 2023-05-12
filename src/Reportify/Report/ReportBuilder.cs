using Reportify.Report.ManicTime;

namespace Reportify.Report;

internal class ReportBuilder : IReportBuilder
{
  private readonly IActivityQuery _activityQuery;

  public ReportBuilder(IActivityQuery activityQuery)
  {
    _activityQuery = activityQuery;
  }

  public async Task<Report> BuildAsync(DateOnly date)
  {
    var dailyReport = await BuildDailyReportAsync(date);
    return new Report(new[] { dailyReport });
  }

  public async Task<Report> BuildAsync(DateOnly startDate, DateOnly endDate)
  {
    var dailyReportTasks = Enumerable
      .Range(0, endDate.DayNumber - startDate.DayNumber + 1)
      .Select(startDate.AddDays)
      .Select(BuildDailyReportAsync);

    var dailyReports = await Task.WhenAll(dailyReportTasks);
    return new Report(dailyReports);
  }

  private async Task<DailyReport> BuildDailyReportAsync(DateOnly date)
  {
    var activities = await _activityQuery.GetAsync(date);
    var positions = activities.Select(a => new Position(a.Name, TimeSpan.FromSeconds(a.TotalSeconds), 123456));
    return new DailyReport(date, positions);
  }
}