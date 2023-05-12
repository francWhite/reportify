namespace Reportify.Report;

internal interface IErpPositionEvaluator
{
  Task<int?> EvaluateAsync(string activityName, CancellationToken cancellationToken = default);
}