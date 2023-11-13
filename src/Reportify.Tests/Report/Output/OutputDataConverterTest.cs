using FluentAssertions.Execution;
using Reportify.Report;
using Reportify.Report.Output;

namespace Reportify.Tests.Report.Output;

public class OutputDataConverterTest
{
  private readonly OutputDataConverter _sut = new();

  [Fact]
  public void Convert_WhenNoDailyReportsExists_ThenReturnEmptyOutputData()
  {
    var report = new Reportify.Report.Report(new List<DailyReport>());

    var result = _sut.Convert(report);

    result.DailySummaries.Should().BeEmpty();
  }

  [Fact]
  public void Convert_WhenNoPositionsExists_ThenReturnEmptyOutputData()
  {
    var report = new Reportify.Report.Report(
      new List<DailyReport>
      {
        new(new DateOnly(2023, 1, 1), new List<Position>()),
        new(new DateOnly(2023, 1, 2), new List<Position>()),
      });

    var result = _sut.Convert(report);

    result.DailySummaries.Should().BeEmpty();
  }

  [Fact]
  public void Convert_WhenOnePositionExists_ThenReturnOneDailySummary()
  {
    var report = new ReportBuilder()
      .WithPosition(new DateOnly(2023, 1, 1), "Position 1", TimeSpan.FromHours(1), 1)
      .Build();

    var result = _sut.Convert(report);

    result.DailySummaries.Should().HaveCount(1);
  }

  [Fact]
  public void Convert_WhenMultiplePositionExists_ThenPositionsAreGroupedByDate()
  {
    var dateA = new DateOnly(2023, 1, 1);
    var dateB = new DateOnly(2023, 1, 2);

    var report = new ReportBuilder()
      .WithPosition(dateA, "Position 1", TimeSpan.FromHours(1), 1)
      .WithPosition(dateA, "Position 2", TimeSpan.FromHours(2), 2)
      .WithPosition(dateB, "Position 3", TimeSpan.FromHours(3), 3)
      .Build();

    var result = _sut.Convert(report);

    using (new AssertionScope())
    {
      result.DailySummaries.Should().HaveCount(2);
      var dailySummaryA = result.DailySummaries.Single(d => d.Date == dateA);
      dailySummaryA.PositionSummaries.Should().HaveCount(2);
      dailySummaryA.PositionSummaries.Should().ContainSingle(p => p.PositionId == 1);
      dailySummaryA.PositionSummaries.Should().ContainSingle(p => p.PositionId == 2);

      var dailySummaryB = result.DailySummaries.Single(d => d.Date == dateB);
      dailySummaryB.PositionSummaries.Should().HaveCount(1);
      dailySummaryB.PositionSummaries.Should().ContainSingle(p => p.PositionId == 3);
    }
  }

  [Fact]
  public void Convert_WhenMultipleActivitiesExists_ThenActivitiesAreGroupedByPosition()
  {
    var date = new DateOnly(2023, 1, 1);

    var report = new ReportBuilder()
      .WithPosition(date, "Position 1A", TimeSpan.FromMinutes(10), 1)
      .WithPosition(date, "Position 1B", TimeSpan.FromMinutes(10), 1)
      .WithPosition(date, "Position 2A", TimeSpan.FromMinutes(15), 2)
      .WithPosition(date, "Position 2B", TimeSpan.FromMinutes(15), 2)
      .WithPosition(date, "no position A", TimeSpan.FromMinutes(15), null)
      .WithPosition(date, "no position B", TimeSpan.FromMinutes(15), null)
      .Build();

    var result = _sut.Convert(report);

    using (new AssertionScope())
    {
      var positionSummaries = result.DailySummaries.SingleOrDefault()?.PositionSummaries.ToList();
      var positionSummary1 = positionSummaries?.SingleOrDefault(d => d.PositionId == 1);

      positionSummary1.Should().NotBeNull();
      positionSummary1!.Activities.Should().ContainSingle(a => a.Name == "Position 1A");
      positionSummary1.Activities.Should().ContainSingle(a => a.Name == "Position 1B");

      var positionSummary2 = positionSummaries?.SingleOrDefault(d => d.PositionId == 2);
      positionSummary2.Should().NotBeNull();
      positionSummary2!.Activities.Should().ContainSingle(a => a.Name == "Position 2A");
      positionSummary2.Activities.Should().ContainSingle(a => a.Name == "Position 2B");

      var positionSummary3 = positionSummaries?.SingleOrDefault(d => d.PositionId == null);
      positionSummary3.Should().NotBeNull();
      positionSummary3!.Activities.Should().ContainSingle(a => a.Name == "no position A");
      positionSummary3.Activities.Should().ContainSingle(a => a.Name == "no position B");
    }
  }

  [Fact]
  public void Convert_WhenMultiplePositionExists_ThenSummaryIsRoundedCorrectly()
  {
    var date = new DateOnly(2023, 1, 1);

    var report = new ReportBuilder()
      .WithPosition(date, "Position 1", TimeSpan.FromMinutes(14), 1)
      .WithPosition(date, "Position 1", TimeSpan.FromMinutes(27), 1) //14 + 27 = 41 -> 0.75
      .WithPosition(date, "Position 2", TimeSpan.FromMinutes(7), 2)
      .WithPosition(date, "Position 2", TimeSpan.FromMinutes(15), 2) //7 + 15 = 22 -> 0.25
      .WithPosition(date, "Position 3", TimeSpan.FromMinutes(19), 3) //19 -> 0.25
      .WithPosition(date, "no erp position", TimeSpan.FromMinutes(37), null) //37 -> 0.5
      .Build();

    var result = _sut.Convert(report);

    using (new AssertionScope())
    {
      var dailySummary = result.DailySummaries.SingleOrDefault();
      dailySummary.Should().NotBeNull();
      dailySummary!.TotalDuration.Should().Be(TimeSpan.FromMinutes(119));
      dailySummary.SumOfRoundedDurationsInHours.Should().Be(1.75);

      var positionSummary1 = dailySummary.PositionSummaries.SingleOrDefault(d => d.PositionId == 1);
      positionSummary1.Should().NotBeNull();
      positionSummary1!.Duration.Should().Be(TimeSpan.FromMinutes(41));
      positionSummary1.RoundedDurationInHours.Should().Be(0.75);

      var positionSummary2 = dailySummary.PositionSummaries.SingleOrDefault(d => d.PositionId == 2);
      positionSummary2.Should().NotBeNull();
      positionSummary2!.Duration.Should().Be(TimeSpan.FromMinutes(22));
      positionSummary2.RoundedDurationInHours.Should().Be(0.25);

      var positionSummary3 = dailySummary.PositionSummaries.SingleOrDefault(d => d.PositionId == 3);
      positionSummary3.Should().NotBeNull();
      positionSummary3!.Duration.Should().Be(TimeSpan.FromMinutes(19));
      positionSummary3.RoundedDurationInHours.Should().Be(0.25);

      var positionSummary4 = dailySummary.PositionSummaries.SingleOrDefault(d => d.PositionId == null);
      positionSummary4.Should().NotBeNull();
      positionSummary4!.Duration.Should().Be(TimeSpan.FromMinutes(37));
      positionSummary4.RoundedDurationInHours.Should().Be(0.5);
    }
  }

  [Fact]
  public void Convert_ResultShouldBeSorted()
  {
    var dateA = new DateOnly(2023, 1, 1);
    var dateB = new DateOnly(2023, 1, 2);

    var report = new ReportBuilder()
      .WithPosition(dateB, "Position 3", TimeSpan.FromHours(3), 3)
      .WithPosition(dateA, "Position 1A", TimeSpan.FromHours(1), 1)
      .WithPosition(dateA, "Position 1B", TimeSpan.FromHours(2), 1)
      .WithPosition(dateA, "Position 2A", TimeSpan.FromHours(3), 2)
      .WithPosition(dateA, "Position 2B", TimeSpan.FromHours(4), 2)
      .Build();

    var result = _sut.Convert(report);

    using (new AssertionScope())
    {
      result.DailySummaries.Should().HaveCount(2);
      result.DailySummaries.First().Date.Should().Be(dateA);
      result.DailySummaries.Last().Date.Should().Be(dateB);

      var positionSummaries = result.DailySummaries.First().PositionSummaries.ToList();
      positionSummaries.Should().HaveCount(2);
      positionSummaries.First().PositionId.Should().Be(2);
      positionSummaries.Last().PositionId.Should().Be(1);

      var activities1 = positionSummaries.First().Activities.ToList();
      activities1.Should().HaveCount(2);
      activities1.First().Name.Should().Be("Position 2B");
      activities1.Last().Name.Should().Be("Position 2A");

      var activities2 = positionSummaries.Last().Activities.ToList();
      activities2.Should().HaveCount(2);
      activities2.First().Name.Should().Be("Position 1B");
      activities2.Last().Name.Should().Be("Position 1A");
    }
  }
}