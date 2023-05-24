using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Reportify.Configuration;

namespace Reportify.Report.Jira;

internal class JiraService : IJiraService
{
  private readonly HttpClient _httpClient;

  public JiraService(HttpClient httpClient, IOptions<JiraOptions> options)
  {
    _httpClient = httpClient;
    _httpClient.BaseAddress = new Uri($"{options.Value.Url}/rest/api/2/issue/");
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AccessToken);
  }

  public async Task<int?> GetErpPositionByIssueKey(string issueKey, CancellationToken cancellationToken = default)
  {
    var result = await _httpClient.GetAsync(issueKey, cancellationToken);
    if (!result.IsSuccessStatusCode)
    {
      return null;
    }

    var issue = await result.Content.ReadFromJsonAsync<Issue>(cancellationToken: cancellationToken);
    return (int?)issue?.Fields.ErpPosition;
  }
}

file sealed record Issue([property: JsonPropertyName("fields")] Fields Fields);

file sealed record Fields([property: JsonPropertyName("customfield_10701")]
  decimal? ErpPosition);