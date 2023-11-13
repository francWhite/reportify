using Reportify.Report;

namespace Reportify.Tests.Report;

internal class ReportBuilder
{
  private readonly List<DailyReport> _dailyReports = new();

  public Reportify.Report.Report Build() => new(_dailyReports);

  public ReportBuilder WithPosition(DateOnly date, string name, TimeSpan duration, int? positionId)
  {
    var position = new Position(name, duration, positionId);

    var existingDailyReport = _dailyReports.SingleOrDefault(d => d.Date == date);
    if (existingDailyReport is not null)
    {
      var dailyReport = existingDailyReport with { Positions = existingDailyReport.Positions.Append(position) };
      _dailyReports.Remove(existingDailyReport);
      _dailyReports.Add(dailyReport);
    }
    else
    {
      _dailyReports.Add(new DailyReport(date, new List<Position> { position }));
    }

    return this;
  }
}