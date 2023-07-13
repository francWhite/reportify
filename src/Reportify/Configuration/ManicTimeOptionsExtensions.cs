using Microsoft.Data.Sqlite;

namespace Reportify.Configuration;

internal static class ManicTimeOptionsExtensions
{
  public static string BuildConnectionString(this ManicTimeOptions options)
  {
    var builder = new SqliteConnectionStringBuilder
    {
      DataSource = options.DatabasePath,
      Mode = SqliteOpenMode.ReadOnly
    };

    return builder.ConnectionString;
  }
}