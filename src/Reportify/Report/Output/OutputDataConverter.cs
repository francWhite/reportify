﻿using Reportify.Extensions;

namespace Reportify.Report.Output;

internal class OutputDataConverter : IOutputDataConverter
{
  public OutputData Convert(Report report)
  {
    var dailySummaries = report.DailyReports
      .Where(d => d.Positions.Any())
      .Select(ConvertToDayGroup)
      .OrderBy(d => d.Date);

    return new OutputData(dailySummaries);
  }

  private static DailySummary ConvertToDayGroup(DailyReport dailyReport)
  {
    var positionGroups = ConvertToPositionGroup(dailyReport.Positions)
      .OrderByDescending(p => p.Duration)
      .ToList();

    return new DailySummary(
      dailyReport.Date,
      positionGroups.Sum(p => p.RoundedDurationInHours),
      positionGroups.Sum(p => p.Duration),
      positionGroups);
  }

  private static IEnumerable<PositionSummary> ConvertToPositionGroup(IEnumerable<Position> positions)
  {
    return positions
      .GroupBy(p => p.ErpPositionId)
      .Select(
        group => new PositionSummary(
          group.Key,
          group.Sum(p => p.Duration),
          group.Sum(p => p.Duration).RoundToQuarterHours(),
          string.Join(", ", group.Select(p => p.Note).Where(n => !string.IsNullOrWhiteSpace(n))),
          group.Select(p => new Activity(p.Name, p.Duration)).OrderByDescending(a => a.Duration)));
  }
}