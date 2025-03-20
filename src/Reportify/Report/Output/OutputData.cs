namespace Reportify.Report.Output;

internal record OutputData(IEnumerable<DailySummary> DailySummaries);

internal record DailySummary(DateOnly Date, double SumOfRoundedDurationsInHours, TimeSpan TotalDuration,
  IEnumerable<PositionSummary> PositionSummaries);

internal record PositionSummary(int? PositionId, TimeSpan Duration, double RoundedDurationInHours, string? Notes,
  IEnumerable<Activity> Activities);

internal record Activity(string Name, TimeSpan Duration);