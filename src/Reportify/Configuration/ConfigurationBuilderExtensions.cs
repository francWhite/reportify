using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Reportify.Configuration;

internal static class ConfigurationBuilderExtensions
{
  private const string DefaultDatabasePath = "Finkit\\ManicTime\\ManicTimeReports.db";
  private const string DefaultJiraUrl = "https://jira.atlassian.com";

  public static IConfigurationBuilder AddReportifyConfiguration(this IConfigurationBuilder builder)
  {
    if (!File.Exists(ConfigurationFileInfo.FilePath))
    {
      CreateDefaultConfiguration(ConfigurationFileInfo.FilePath);
    }

    var fileProvider = new PhysicalFileProvider(ConfigurationFileInfo.FolderPath, ExclusionFilters.None);
    return builder.AddYamlFile(fileProvider, ConfigurationFileInfo.FileName, false, false);
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