namespace Reportify;

internal static class EnumerableExtension
{
  public static TimeSpan Sum(this IEnumerable<TimeSpan> source)
  {
    return source.Aggregate((sum, current) => sum.Add(current));
  }

  public static TimeSpan Sum<T>(this IEnumerable<T> source, Func<T, TimeSpan> selector)
  {
    return source.Select(selector).Sum();
  }
}