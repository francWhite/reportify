namespace Reportify;

internal static class DateOnlyExtensions
{
  public static DateOnly GetFirstDayOfWeek(this DateOnly date)
  {
    return date.AddDays(-date.DayNumber % 7);
  }

  public static DateOnly GetLastDayOfWeek(this DateOnly date)
  {
    return date.GetFirstDayOfWeek().AddDays(6);
  }
}