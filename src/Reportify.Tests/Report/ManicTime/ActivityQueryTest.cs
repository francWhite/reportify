using Microsoft.Extensions.Options;
using Reportify.Configuration;
using Reportify.Report.ManicTime;

namespace Reportify.Tests.Report.ManicTime;

public class ActivityQueryTest
{
  private readonly ActivityQuery _sut;

  public ActivityQueryTest()
  {
    var options = new ReportifyOptions
    {
      JiraUrl = string.Empty,
      ManicTimeDatabasePath =  Path.Combine(Environment.CurrentDirectory, "Report/ManicTime/manictime.db")
    };

    _sut = new ActivityQuery(Options.Create(options));
  }

  [Fact]
  public async Task GetAsync_ReturnsActivities()
  {
    var date = new DateOnly(2023, 5, 12);
    var activities = await _sut.GetAsync(date);

    activities.Should().BeEquivalentTo(
      new[]
      {
        new Activity("Activity 1 $123456", 9462),
        new Activity("Activity 2 #TEST-2", 5847),
        new Activity("Activity 3 #TEST-3", 3466),
        new Activity("Activity 4", 975),
      });
  }
}