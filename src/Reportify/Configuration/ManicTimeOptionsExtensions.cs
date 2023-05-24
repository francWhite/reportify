using Microsoft.Data.Sqlite;

namespace Reportify.Configuration;

internal static class ManicTimeOptionsExtensions
{
  public static string BuildConnectionString(this ManicTimeOptions options)
  {
    //TODO throw if file doesn't exists
    var builder = new SqliteConnectionStringBuilder
    {
      DataSource = options.DatabasePath,
      Mode = SqliteOpenMode.ReadOnly
    };

    return builder.ConnectionString;
  }
}