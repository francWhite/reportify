using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Reportify.Configuration.Validation;

internal class ConfigurationValidator : IConfigurationValidator
{
  private readonly HttpClient _httpClient;
  private readonly ManicTimeOptions _manicTimeOptions;
  private readonly JiraOptions _jiraOptions;

  public ConfigurationValidator(IHttpClientFactory httpClientFactory,
    IOptions<ManicTimeOptions> manicTimeOptions,
    IOptions<JiraOptions> jiraOptions)
  {
    _httpClient = httpClientFactory.CreateClient(HttpClients.Jira);
    _manicTimeOptions = manicTimeOptions.Value;
    _jiraOptions = jiraOptions.Value;
  }

  public async Task<IReadOnlyList<ValidationError>> ValidateAsync()
  {
    var validationErrors = new List<ValidationError?>();
    validationErrors.AddRange(ValidateJiraOptions(_jiraOptions.Url, _jiraOptions.AccessToken));
    validationErrors.Add(await ValidateJiraAccessAsync(_jiraOptions.Url, _jiraOptions.AccessToken));
    validationErrors.Add(ValidateManicTimeOptions(_manicTimeOptions.DatabasePath));

    return validationErrors
      .OfType<ValidationError>()
      .ToList();
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

  private async Task<ValidationError?> ValidateJiraAccessAsync(string url, string accessToken)
  {
    if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(accessToken))
    {
      return null;
    }

    try
    {
      var result = await _httpClient.GetAsync("mypermissions");
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
}

file sealed record Permissions([property: JsonPropertyName("permissions")]
  BrowseProjectsPermission BrowseProjectsPermission);

file sealed record BrowseProjectsPermission([property: JsonPropertyName("BROWSE_PROJECTS")]
  Permission Permission);

file sealed record Permission([property: JsonPropertyName("havePermission")]
  bool HavePermission);