using Reportify.Extensions;

namespace Reportify.Tests.Extensions;

public class TimeSpanExtensionsTest
{
  [Theory]
  [InlineData("01:00", 1.0)]
  [InlineData("01:07", 1.0)]
  [InlineData("01:08", 1.25)]
  [InlineData("01:15", 1.25)]
  [InlineData("01:22", 1.25)]
  [InlineData("01:23", 1.5)]
  [InlineData("01:37", 1.5)]
  [InlineData("01:38", 1.75)]
  [InlineData("01:52", 1.75)]
  [InlineData("01:53", 2.0)]
  public void RoundToQuarterHours(string input, double roundedHours)
  {
    var timeSpan = TimeSpan.Parse(input);
    var result = timeSpan.RoundToQuarterHours();

    result.Should().Be(roundedHours);
  }
}