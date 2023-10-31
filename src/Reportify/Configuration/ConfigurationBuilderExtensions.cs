using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Reportify.Configuration;

internal static class ConfigurationBuilderExtensions
{
  public static IConfigurationBuilder AddReportifyConfiguration(this IConfigurationBuilder builder, string filePath)
  {
    if (!File.Exists(filePath)) CreateDefaultConfiguration(filePath);

    var directoryPath = Path.GetDirectoryName(filePath) ?? ConfigurationFileInfo.FolderPath;

    var fileProvider = new PhysicalFileProvider(directoryPath, ExclusionFilters.None);
    return builder.AddYamlFile(fileProvider, Path.GetFileName(filePath), false, false);
  }

  private static void CreateDefaultConfiguration(string filePath)
  {
    var options = new
    {
      ManicTime = new ManicTimeOptions { DatabasePath = null! },
      Jira = new JiraOptions { Url = null!, AccessToken = null! }
    };

    var yaml = new SerializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build()
      .Serialize(options);

    File.WriteAllText(filePath, yaml);
  }
}