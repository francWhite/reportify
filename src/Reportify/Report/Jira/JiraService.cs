using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Reportify.Configuration;

namespace Reportify.Report.Jira;

internal class JiraService : IJiraService
{
  private readonly HttpClient _httpClient;

  public JiraService(HttpClient httpClient, IOptions<ReportifyOptions> options)
  {
    _httpClient = httpClient;
    _httpClient.BaseAddress = new Uri($"{options.Value.JiraUrl}/rest/api/2/issue/");
  }

  public async Task<int?> GetErpPositionByIssueKey(string issueKey, CancellationToken cancellationToken = default)
  {
    var test = await _httpClient.GetAsync(issueKey, cancellationToken);
    if (!test.IsSuccessStatusCode)
    {
      return null;
    }

    var issue = await test.Content.ReadFromJsonAsync<Issue>(cancellationToken: cancellationToken);
    return issue?.Fields.ErpPosition;
  }
}

file sealed record Issue([property: JsonPropertyName("fields")] Fields Fields);

file sealed record Fields([property: JsonPropertyName("customfield_10701")]
  int? ErpPosition);