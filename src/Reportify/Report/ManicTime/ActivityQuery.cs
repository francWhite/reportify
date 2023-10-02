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

    var selectCommand = BuildCommand(date, cancellationToken);
    var activities = await connection.QueryAsync<Activity>(selectCommand);

    return activities.ToList();
  }

  private static CommandDefinition BuildCommand(DateOnly date, CancellationToken cancellationToken)
  {
    return new CommandDefinition(
      Queries.SelectActivitiesQuery,
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
}