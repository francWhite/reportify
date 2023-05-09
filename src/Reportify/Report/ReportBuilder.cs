namespace Reportify.Report;

internal class ReportBuilder : IReportBuilder
{
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

  private static Task<DailyReport> BuildDailyReportAsync(DateOnly date)
  {
    return Task.FromResult(
      new DailyReport(
        date,
        new[]
        {
          new Position("Position 1", TimeSpan.FromMinutes(240), 456000),
          new Position("Position asdasd 2", TimeSpan.FromMinutes(245), 456000),
          new Position("Position 3", TimeSpan.FromMinutes(90), 456000),
          new Position("Position lll 4", TimeSpan.FromMinutes(14), 456000),
          new Position("Position 5", TimeSpan.FromMinutes(15), null),
          new Position("Position 6", TimeSpan.FromMinutes(7), 989888),
          new Position("Position 7", TimeSpan.FromMinutes(8), 345432),
          new Position("Some very very long text 8", TimeSpan.FromMinutes(8), null)
        }));
  }
}