using System.Globalization;

namespace Reportify.Tests;

public class DateOnlyExtensionsTest
{
  [Theory]
  [InlineData("01.01.2023", "26.12.2022")]
  [InlineData("02.01.2023", "02.01.2023")]
  [InlineData("03.01.2023", "02.01.2023")]
  [InlineData("04.01.2023", "02.01.2023")]
  [InlineData("05.01.2023", "02.01.2023")]
  [InlineData("06.01.2023", "02.01.2023")]
  [InlineData("07.01.2023", "02.01.2023")]
  [InlineData("08.01.2023", "02.01.2023")]
  [InlineData("09.01.2023", "09.01.2023")]
  [InlineData("26.02.2023", "20.02.2023")]
  [InlineData("27.02.2023", "27.02.2023")]
  [InlineData("05.03.2023", "27.02.2023")]
  [InlineData("06.03.2023", "06.03.2023")]
  public void GetFirstDayOfWeekShouldReturnCorrectDate(string dateStr, string expectedDateStr)
  {
    var date = ParseDateOnly(dateStr);
    var expectedDate = ParseDateOnly(expectedDateStr);

    var result = date.GetFirstDayOfWeek();
    result.Should().Be(expectedDate);
  }

  [Theory]
  [InlineData("01.01.2023", "01.01.2023")]
  [InlineData("02.01.2023", "08.01.2023")]
  [InlineData("03.01.2023", "08.01.2023")]
  [InlineData("04.01.2023", "08.01.2023")]
  [InlineData("05.01.2023", "08.01.2023")]
  [InlineData("06.01.2023", "08.01.2023")]
  [InlineData("07.01.2023", "08.01.2023")]
  [InlineData("08.01.2023", "08.01.2023")]
  [InlineData("09.01.2023", "15.01.2023")]
  [InlineData("26.02.2023", "26.02.2023")]
  [InlineData("27.02.2023", "05.03.2023")]
  [InlineData("05.03.2023", "05.03.2023")]
  [InlineData("06.03.2023", "12.03.2023")]
  public void GetLastDayOfWeekShouldReturnCorrectDate(string dateStr, string expectedDateStr)
  {
    var date = ParseDateOnly(dateStr);
    var expectedDate = ParseDateOnly(expectedDateStr);

    var result = date.GetLastDayOfWeek();
    result.Should().Be(expectedDate);
  }

  private static DateOnly ParseDateOnly(string date)
  {
    return DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
  }
}