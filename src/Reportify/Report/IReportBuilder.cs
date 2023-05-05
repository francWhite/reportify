namespace Reportify.Report;

internal interface IReportBuilder
{
  Task<Report> BuildAsync(DateOnly date);
  Task<Report> BuildAsync(DateOnly startDate, DateOnly endDate);
}