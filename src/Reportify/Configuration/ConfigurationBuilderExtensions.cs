using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Reportify.Configuration;

internal static class ConfigurationBuilderExtensions
{
  private const string FileName = ".reportify";
  private const string DefaultDatabasePath = "Finkit\\ManicTime\\ManicTimeReports.db";
  private const string DefaultJiraUrl = "https://jira.atlassian.com";

  public static IConfigurationBuilder AddReportifyConfiguration(this IConfigurationBuilder builder)
  {
    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var filePath = Path.Combine(folderPath, FileName);
    if (!File.Exists(filePath))
    {
      CreateDefaultConfiguration(filePath);
    }

    var fileProvider = new PhysicalFileProvider(folderPath, ExclusionFilters.None);
    return builder.AddYamlFile(fileProvider, FileName, false, false);
  }

  private static void CreateDefaultConfiguration(string filePath)
  {
    var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var databasePath = Path.Combine(localAppDataFolder, DefaultDatabasePath);

    var options = new
    {
      ManicTime = new ManicTimeOptions { DatabasePath = databasePath },
      Jira = new JiraOptions { Url = DefaultJiraUrl, AccessToken = null! }
    };

    var yaml = new SerializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build()
      .Serialize(options);

    File.WriteAllText(filePath, yaml);
  }
}