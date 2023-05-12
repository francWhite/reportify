namespace Reportify.Report;

internal interface IReportBuilder
{
  Task<Report> BuildAsync(DateOnly date, CancellationToken cancellationToken = default);
  Task<Report> BuildAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
}