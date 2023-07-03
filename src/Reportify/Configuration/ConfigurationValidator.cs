using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Reportify.Configuration;

internal class ConfigurationValidator : IConfigurationValidator
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ManicTimeOptions _manicTimeOptions;
  private readonly JiraOptions _jiraOptions;

  private sealed record ValidationError(string Message);

  public ConfigurationValidator(IHttpClientFactory httpClientFactory,
    IOptions<ManicTimeOptions> manicTimeOptions,
    IOptions<JiraOptions> jiraOptions)
  {
    _httpClientFactory = httpClientFactory;
    _manicTimeOptions = manicTimeOptions.Value;
    _jiraOptions = jiraOptions.Value;
  }

  public async Task ValidateAsync()
  {
    //TODO: extract progress writer to reduce duplication
    await AnsiConsole
      .Progress()
      .AutoClear(true)
      .Columns(new TaskDescriptionColumn(), new SpinnerColumn())
      .StartAsync(
        async context =>
        {
          var task = context.AddTask("Validating configuration...").IsIndeterminate();
          task.StartTask();

          var validationErrors = await ValidateConfigurationAsync();
          task.StopTask();

          WriteValidationErrors(validationErrors);
        });
  }

  private async Task<IReadOnlyList<ValidationError>> ValidateConfigurationAsync()
  {
    var validationErrors = new List<ValidationError?>();
    validationErrors.Add(ValidateManicTimeOptions(_manicTimeOptions.DatabasePath));
    validationErrors.AddRange(ValidateJiraOptions(_jiraOptions.Url, _jiraOptions.AccessToken));
    validationErrors.Add(await ValidateJiraAccess(_jiraOptions.Url, _jiraOptions.AccessToken));
    return validationErrors.OfType<ValidationError>().ToList();
  }

  private static ValidationError? ValidateManicTimeOptions(string databasePath)
  {
    if (string.IsNullOrWhiteSpace(databasePath))
    {
      return new ValidationError("ManicTime database path is not defined");
    }

    return !File.Exists(databasePath)
      ? new ValidationError($"ManicTime database file not found: {databasePath}")
      : null;
  }

  private static IEnumerable<ValidationError> ValidateJiraOptions(string url, string accessToken)
  {
    if (string.IsNullOrWhiteSpace(url))
    {
      yield return new ValidationError("Jira URL is not defined");
    }

    if (string.IsNullOrWhiteSpace(accessToken))
    {
      yield return new ValidationError("Jira access token is not defined");
    }
  }

  private async Task<ValidationError?> ValidateJiraAccess(string url, string accessToken)
  {
    if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(accessToken))
    {
      return null;
    }

    try
    {
      using var httpClient = _httpClientFactory.CreateClient(HttpClients.Jira);
      var result = await httpClient.GetAsync("mypermissions");
      if (result.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
      {
        return new ValidationError("Jira access token is invalid");
      }

      var permissions = await result.Content.ReadFromJsonAsync<Permissions>();
      return permissions?.BrowseProjectsPermission.Permission.HavePermission != true
        ? new ValidationError("Provided access token doesn't have permission to browse projects and issues")
        : null;
    }
    catch (Exception e) when (e is HttpRequestException or UriFormatException)
    {
      return new ValidationError($"Jira URL is invalid or can't be reached: {url}");
    }
  }

  private static void WriteValidationErrors(IReadOnlyList<ValidationError> validationErrors)
  {
    if (!validationErrors.Any())
    {
      return;
    }

    var errorMessages = validationErrors
      .Select(v => new Padder(new Text($"- {v.Message}"), new Padding(2, 0)))
      .ToList();

    AnsiConsole.MarkupLine(
      $"[red]Configuration file [underline]{ConfigurationFileInfo.FilePath}[/] is invalid, please fix the following errors.[/]");
    AnsiConsole.Write(new Rows(errorMessages));
    Environment.Exit(1);
  }
}

file sealed record Permissions([property: JsonPropertyName("permissions")]
  BrowseProjectsPermission BrowseProjectsPermission);

file sealed record BrowseProjectsPermission([property: JsonPropertyName("BROWSE_PROJECTS")]
  Permission Permission);

file sealed record Permission([property: JsonPropertyName("havePermission")]
  bool HavePermission);