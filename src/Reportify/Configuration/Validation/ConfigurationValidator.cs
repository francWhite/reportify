using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Reportify.Configuration.Validation;

internal class ConfigurationValidator : IConfigurationValidator
{
  private readonly Lazy<HttpClient> _httpClient;
  private readonly ManicTimeOptions _manicTimeOptions;
  private readonly JiraOptions _jiraOptions;

  public ConfigurationValidator(IHttpClientFactory httpClientFactory,
    IOptions<ManicTimeOptions> manicTimeOptions,
    IOptions<JiraOptions> jiraOptions)
  {
    _httpClient = new Lazy<HttpClient>(() => httpClientFactory.CreateClient(HttpClients.Jira));
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
      var result = await _httpClient.Value.GetAsync("mypermissions");
      if (result.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
      {
        return new ValidationError("Jira access token is invalid");
      }

      var contentTypeIsJson = result.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json;
      var requestHasBeenRedirected = HostsAreEqual(result.RequestMessage?.RequestUri, new Uri(url)) == false;
      if (contentTypeIsJson == false && requestHasBeenRedirected)
      {
        var unescapedUri = Uri.UnescapeDataString(result.RequestMessage?.RequestUri?.ToString() ?? string.Empty);
        return new ValidationError(
          $"Redirect with unexpected content type detected, check if Jira URL is correct or any additional authentication is required. Request Uri: {unescapedUri}");
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

  private static bool HostsAreEqual(Uri? uri1, Uri? uri2) =>
    Uri.Compare(
      uri1,
      uri2,
      UriComponents.Host,
      UriFormat.Unescaped,
      StringComparison.OrdinalIgnoreCase) == 0;
}