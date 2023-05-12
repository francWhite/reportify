namespace Reportify.Report.Jira;

internal class JiraService : IJiraService
{
  public Task<int?> GetErpPositionByIssueKey(string issueKey, CancellationToken cancellationToken = default)
  {
    return Task.FromResult<int?>(999999);
  }
}