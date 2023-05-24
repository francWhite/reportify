using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Reportify.Configuration;

namespace Reportify.Report.ManicTime;

internal class ActivityQuery : IActivityQuery
{
  private readonly ManicTimeOptions _options;

  public ActivityQuery(IOptions<ManicTimeOptions> options)
  {
    _options = options.Value;
  }

  public async Task<IReadOnlyList<Activity>> GetAsync(DateOnly date,
    CancellationToken cancellationToken = default)
  {
    await using var connection = new SqliteConnection(_options.BuildConnectionString());

    // workaround because dapper interprets the sum-column type as an byte[] instead of a long if
    // no rows are returned and finds no matching Activity constructor, thus throwing an exception
    var countCommand = BuildCommand(Queries.CountActivitiesQuery, date, cancellationToken);
    var activitiesCount = await connection.QuerySingleAsync<int>(countCommand);
    if (activitiesCount == 0)
    {
      return new List<Activity>();
    }

    var selectCommand = BuildCommand(Queries.SelectActivitiesQuery, date, cancellationToken);
    var activities = await connection.QueryAsync<Activity>(selectCommand);
    return activities.ToList();
  }

  private static CommandDefinition BuildCommand(string query, DateOnly date, CancellationToken cancellationToken)
  {
    return new CommandDefinition(
      query,
      new
      {
        startOfDay = date.ToDateTime(TimeOnly.MinValue),
        startOfNextDay = date.AddDays(1).ToDateTime(TimeOnly.MinValue)
      },
      cancellationToken: cancellationToken);
  }
}

file static class Queries
{
  public const string SelectActivitiesQuery = """
      SELECT grp.name          AS name,
           SUM(totalseconds) AS totalseconds
      FROM ar_taglistbyday tag
           JOIN ar_commongrouplist grp ON tag.commonid = grp.commonid
      WHERE tag.hour > @startOfDay
        AND tag.hour < @startOfNextDay
      GROUP BY tag.commonid;
      """;

  public const string CountActivitiesQuery = """
      SELECT COUNT(*)
      FROM ar_taglistbyday tag
           JOIN ar_commongrouplist grp ON tag.commonid = grp.commonid
      WHERE tag.hour > @startOfDay
        AND tag.hour < @startOfNextDay;
    """;
}