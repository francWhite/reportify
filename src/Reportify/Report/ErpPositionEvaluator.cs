using System.Text.RegularExpressions;
using Reportify.Report.Jira;

namespace Reportify.Report;

internal partial class ErpPositionEvaluator : IErpPositionEvaluator
{
  private readonly IJiraService _jiraService;

  public ErpPositionEvaluator(IJiraService jiraService)
  {
    _jiraService = jiraService;
  }

  public async Task<int?> EvaluateAsync(string activityName, CancellationToken cancellationToken = default)
  {
    var erpPositionMatch = ErpPositionRegex().Match(activityName);
    if (erpPositionMatch.Success)
    {
      return int.Parse(erpPositionMatch.Groups["position"].Value);
    }

    var issueKeyMatch = IssueKeyRegex().Match(activityName);
    if (issueKeyMatch.Success)
    {
      return await _jiraService.GetErpPositionByIssueKey(issueKeyMatch.Groups["key"].Value, cancellationToken);
    }

    return null;
  }

  [GeneratedRegex("\\$(?<position>\\d{6})")]
  private static partial Regex ErpPositionRegex();

  [GeneratedRegex("#(?<key>[a-zA-Z]+-\\d+)")]
  private static partial Regex IssueKeyRegex();
}