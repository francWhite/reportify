namespace Reportify.Report;

internal interface ICreateReportCommand
{
  Task CreateForDateAsync(DateOnly date);
  Task CreateForDateRangeAsync(DateOnly startDate, DateOnly endDate);
}