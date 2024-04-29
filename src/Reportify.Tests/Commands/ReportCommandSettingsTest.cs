using Reportify.Commands;

namespace Reportify.Tests.Commands;

public class ReportCommandSettingsTest
{
  [Fact]
  public void Validate_WhenDateAndEntireWeekAreSet_ShouldReturnError()
  {
    var settings = new ReportCommandSettings
    {
      Date = DateOnly.FromDateTime(DateTime.Now),
      EntireWeek = true
    };

    var result = settings.Validate();

    result.Successful.Should().BeFalse();
    result.Message.Should().Be("date and week options are mutually exclusive");
  }

  [Fact]
  public void Validate_WhenWeekOffsetIsSetWithoutEntireWeek_ShouldReturnError()
  {
    var settings = new ReportCommandSettings
    {
      WeekOffset = 1,
      EntireWeek = false
    };

    var result = settings.Validate();

    result.Successful.Should().BeFalse();
    result.Message.Should().Be("week offset can only be used with the week option");
  }
}