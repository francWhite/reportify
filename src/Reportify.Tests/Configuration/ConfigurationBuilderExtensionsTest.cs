using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Reportify.Configuration;

namespace Reportify.Tests.Configuration;

public class ConfigurationBuilderExtensionsTest
{
  [Fact]
  public void AddReportifyConfiguration_WhenNoFileExists_ThenDefaultConfigurationIsCreated()
  {
    using var configFile = new TempFile();

    var (jiraOptions, manicTimeOptions) = BuildAndGetOptions(configFile.FilePath);

    using (new AssertionScope())
    {
      jiraOptions.Should().NotBeNull();
      jiraOptions?.Url.Should().Be(string.Empty);
      jiraOptions?.AccessToken.Should().Be(string.Empty);
      manicTimeOptions.Should().NotBeNull();
      manicTimeOptions?.DatabasePath.Should().Contain(@"Finkit\ManicTime\ManicTimeReports.db");
    }
  }

  [Fact]
  public void AddReportifyConfiguration_WhenFileExists_ThenConfigurationIsRead()
  {
    var configFieContent = /*lang=yaml*/
      """
      manicTime:
        databasePath: database.db
      jira:
        url: jira.url
        accessToken: token
      """;

    using var configFile = new TempFile().Create(configFieContent);

    var (jiraOptions, manicTimeOptions) = BuildAndGetOptions(configFile.FilePath);

    using (new AssertionScope())
    {
      jiraOptions.Should().NotBeNull();
      jiraOptions?.Url.Should().Be("jira.url");
      jiraOptions?.AccessToken.Should().Be("token");
      manicTimeOptions.Should().NotBeNull();
      manicTimeOptions?.DatabasePath.Should().Be("database.db");
    }
  }

  [Fact]
  public void AddReportifyConfiguration_WhenFileExistsButIsInvalidFormat_ThenExceptionIsThrown()
  {
    var act = () =>
    {
      using var configFile = new TempFile().Create("no valid format");
      return BuildAndGetOptions(configFile.FilePath);
    };

    act.Should().Throw<InvalidDataException>();
  }

  private static (JiraOptions?, ManicTimeOptions?) BuildAndGetOptions(string filePath)
  {
    var configuration = new ConfigurationBuilder()
      .AddReportifyConfiguration(filePath)
      .Build();

    var jiraOptions = configuration.GetSection("Jira").Get<JiraOptions>();
    var manicTimeOptions = configuration.GetSection("ManicTime").Get<ManicTimeOptions>();

    return (jiraOptions, manicTimeOptions);
  }
}