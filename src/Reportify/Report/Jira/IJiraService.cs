namespace Reportify.Report.Jira;

internal interface IJiraService
{
  Task<int?> GetErpPositionByIssueKey(string issueKey, CancellationToken cancellationToken = default);
}