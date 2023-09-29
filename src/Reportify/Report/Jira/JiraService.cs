using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Reportify.Report.Jira;

internal class JiraService : IJiraService
{
  private readonly Lazy<HttpClient> _httpClient;

  public JiraService(IHttpClientFactory httpClientFactory)
  {
    _httpClient = new Lazy<HttpClient>(() => httpClientFactory.CreateClient(HttpClients.Jira));
  }

  public async Task<int?> GetErpPositionByIssueKey(string issueKey, CancellationToken cancellationToken = default)
  {
    var result = await _httpClient.Value.GetAsync($"issue/{issueKey}", cancellationToken);
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