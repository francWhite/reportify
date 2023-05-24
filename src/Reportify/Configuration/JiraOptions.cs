namespace Reportify.Configuration;

internal class JiraOptions
{
  public required string Url { get; init; }
  public required string AccessToken { get; init; }
}