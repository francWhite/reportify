namespace Reportify.Report.ManicTime;

internal record Activity(string Name, long TotalSeconds)
{
  // needed because dapper interprets the sum-column type as an byte[] instead of a long if
  // no rows are returned and finds no matching Activity constructor, thus throwing an exception
  public Activity(string name, byte[] totalSeconds)
    : this(name, BitConverter.ToInt64(totalSeconds))
  {
  }
}