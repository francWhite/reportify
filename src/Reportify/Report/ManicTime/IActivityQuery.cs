namespace Reportify.Report.ManicTime;

internal interface IActivityQuery
{
  Task<IReadOnlyList<Activity>> GetAsync(DateOnly date, CancellationToken cancellationToken = default);
}