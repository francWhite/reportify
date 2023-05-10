using Microsoft.Data.Sqlite;

namespace Reportify.Configuration;

internal static class ReportifyOptionsExtensions
{
  public static string BuildConnectionString(this ReportifyOptions options)
  {
    //TODO throw if file doesn't exists
    var dataSource = Environment.ExpandEnvironmentVariables(options.ManicTimeDatabasePath);

    var builder = new SqliteConnectionStringBuilder
    {
      DataSource = dataSource,
      Mode = SqliteOpenMode.ReadOnly
    };

    return builder.ConnectionString;
  }
}