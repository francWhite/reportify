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
}