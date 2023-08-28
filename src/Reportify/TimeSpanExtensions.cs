namespace Reportify;

internal static class TimeSpanExtensions
{
  public static double RoundToQuarterHours(this TimeSpan timeSpan)
  {
    return Math.Round(timeSpan.TotalHours * 4, MidpointRounding.ToEven) / 4;
  }
}