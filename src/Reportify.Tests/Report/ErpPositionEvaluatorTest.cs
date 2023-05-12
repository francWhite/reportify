using Reportify.Report;
using Reportify.Report.Jira;

namespace Reportify.Tests.Report;

public class ErpPositionEvaluatorTest
{
  private readonly ErpPositionEvaluator _sut;
  private readonly IJiraService _jiraService;

  public ErpPositionEvaluatorTest()
  {
    _jiraService = A.Fake<IJiraService>();
    _sut = new ErpPositionEvaluator(_jiraService);
  }

  [Theory]
  [InlineData("Some activity $123456", 123456)]
  [InlineData("Some  $000001 activity", 1)]
  [InlineData("$999000", 999000)]
  public async Task EvaluateAsync_WhenInputContainsErpPosition_ThenParseAndReturnErpPosition(string activityName,
    int expectedErpPosition)
  {
    var result = await _sut.EvaluateAsync(activityName);
    result.Should().Be(expectedErpPosition);
  }

  [Theory]
  [InlineData("Some activity #TEST-123", "TEST-123")]
  [InlineData("Some #C-19 activity", "C-19")]
  [InlineData("#AbC-345", "AbC-345")]
  public async Task EvaluateAsync_WhenInputContainsJiraIdentifier_ThenReturnErpPosition(string activityName,
    string issueKey)
  {
    const int erpPosition = 123456;
    A.CallTo(() => _jiraService.GetErpPositionByIssueKey(issueKey, A<CancellationToken>._))
      .Returns(erpPosition);

    var result = await _sut.EvaluateAsync(activityName);
    result.Should().Be(erpPosition);
  }

  [Theory]
  [InlineData("Some activity $TEST-123")]
  [InlineData("Some activity TEST-123")]
  [InlineData("Some activity #123456")]
  [InlineData("$12345")]
  [InlineData("$ 123456")]
  public async Task EvaluateAsync_WhenInputDoesNotContainErpPosition_ThenReturnNull(string activityName)
  {
    var result = await _sut.EvaluateAsync(activityName);
    result.Should().BeNull();
  }
}