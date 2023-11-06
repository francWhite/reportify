using System.Net;
using System.Text.Json;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using Reportify.Configuration;
using Reportify.Configuration.Validation;
using RichardSzalay.MockHttp;

namespace Reportify.Tests.Configuration.Validation;

public class ConfigurationValidatorTest
{
  private readonly JiraOptions _emptyJiraOptions = new() { Url = string.Empty, AccessToken = string.Empty };
  private readonly ManicTimeOptions _emptyManicTimeOptions = new() { DatabasePath = string.Empty };
  private readonly MockHttpMessageHandler _mockHttp = new();

  [Fact]
  public async Task ValidateAsync_WhenNoOptionsAreDefined_ThenReturnValidationErrorsPerConfiguration()
  {
    var jiraOptions = new JiraOptions { Url = string.Empty, AccessToken = string.Empty };
    var manicTimeOptions = new ManicTimeOptions { DatabasePath = string.Empty };
    var sut = CreateSut(jiraOptions, manicTimeOptions);

    var result = await sut.ValidateAsync();

    using (new AssertionScope())
    {
      result.Should().ContainSingle(v => v.Message == "ManicTime database path is not defined");
      result.Should().ContainSingle(v => v.Message == "Jira URL is not defined");
      result.Should().ContainSingle(v => v.Message == "Jira access token is not defined");
    }
  }

  [Fact]
  public async Task ValidateAsync_WhenManicTimeDatabaseFileDoesNotExist_ThenReturnValidationError()
  {
    var manicTimeOptions = new ManicTimeOptions { DatabasePath = "-1" };
    var sut = CreateSut(_emptyJiraOptions, manicTimeOptions);

    var result = await sut.ValidateAsync();

    result.Should().ContainSingle(v => v.Message == "ManicTime database file not found: -1");
  }

  [Theory]
  [InlineData(HttpStatusCode.Forbidden)]
  [InlineData(HttpStatusCode.Unauthorized)]
  public async Task ValidateAsync_WhenRequestReturnsInvalidStatusCode_ThenReturnValidationError(
    HttpStatusCode statusCode)
  {
    _mockHttp
      .When("*/mypermissions")
      .Respond(statusCode);

    var jiraOptions = new JiraOptions { Url = "https://jira.com", AccessToken = "123" };
    var sut = CreateSut(jiraOptions, _emptyManicTimeOptions);

    var result = await sut.ValidateAsync();

    result.Should().ContainSingle(v => v.Message == "Jira access token is invalid");
  }

  [Fact]
  public async Task ValidateAsync_WhenMediaTypeIsNotJsonAndRedirectIsDetected_ThenReturnValidationError()
  {
    _mockHttp
      .When("*/mypermissions")
      .Respond(
        t =>
        {
          t.RequestUri = new Uri("https://different.com");
          return new StringContent("non json");
        });

    var jiraOptions = new JiraOptions { Url = "https://jira.com", AccessToken = "123" };
    var sut = CreateSut(jiraOptions, _emptyManicTimeOptions);

    var result = await sut.ValidateAsync();
    result.Should().ContainSingle(v => v.Message.Contains("Redirect with unexpected content type detected"));
  }

  [Fact]
  public async Task ValidateAsync_WhenPermissionIsNotPresent_ThenReturnValidationError()
  {
    var response = new Permissions(new BrowseProjectsPermission(new Permission(false)));

    _mockHttp
      .When("*/mypermissions")
      .Respond("application/json", JsonSerializer.Serialize(response));

    var jiraOptions = new JiraOptions { Url = "https://jira.com", AccessToken = "123" };
    var sut = CreateSut(jiraOptions, _emptyManicTimeOptions);

    var result = await sut.ValidateAsync();
    result.Should().ContainSingle(
      v => v.Message == "Provided access token doesn't have permission to browse projects and issues");
  }

  [Theory]
  [InlineData(typeof(HttpRequestException))]
  [InlineData(typeof(UriFormatException))]
  public async Task ValidateAsync_WhenExceptionOccurs_ThenReturnValidationError(Type exceptionType)
  {
    _mockHttp
      .When("*/mypermissions")
      .Throw((Exception)Activator.CreateInstance(exceptionType)!);

    var jiraOptions = new JiraOptions { Url = "https://jira.com", AccessToken = "123" };
    var sut = CreateSut(jiraOptions, _emptyManicTimeOptions);

    var result = await sut.ValidateAsync();
    result.Should().ContainSingle(
      v => v.Message == $"Jira URL is invalid or can't be reached: {jiraOptions.Url}");
  }

  [Fact]
  public async Task ValidateAsync_WhenConfigurationIsValid_ThenNoValidationsAreReturned()
  {
    var response = new Permissions(new BrowseProjectsPermission(new Permission(true)));

    _mockHttp
      .When("*/mypermissions")
      .Respond("application/json", JsonSerializer.Serialize(response));

    var databaseFilePath = Path.GetTempFileName();
    File.Create(databaseFilePath).Close();

    var jiraOptions = new JiraOptions { Url = "https://jira.com", AccessToken = "123" };
    var manicTimeOptions = new ManicTimeOptions { DatabasePath = databaseFilePath };

    var sut = CreateSut(jiraOptions, manicTimeOptions);
    var result = await sut.ValidateAsync();

    result.Should().BeEmpty();
    File.Delete(databaseFilePath);
  }

  private ConfigurationValidator CreateSut(JiraOptions jiraOptions,
    ManicTimeOptions manicTimeOptions)
  {
    var httpClient = _mockHttp.ToHttpClient();

    if (!string.IsNullOrWhiteSpace(jiraOptions.Url))
      httpClient.BaseAddress = new Uri(jiraOptions.Url);

    var httpClientFactory = A.Fake<IHttpClientFactory>();
    A.CallTo(() => httpClientFactory.CreateClient(HttpClients.Jira))
      .Returns(httpClient);

    return new ConfigurationValidator(
      httpClientFactory,
      new OptionsWrapper<ManicTimeOptions>(manicTimeOptions),
      new OptionsWrapper<JiraOptions>(jiraOptions));
  }
}