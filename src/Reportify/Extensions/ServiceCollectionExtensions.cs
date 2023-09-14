using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reportify.Configuration;

namespace Reportify.Extensions;

internal static class ServiceCollectionExtensions
{
  public static IServiceCollection AddJiraHttpClient(this IServiceCollection services)
  {
    services.AddHttpClient(
      HttpClients.Jira,
      (serviceProvider, client) =>
      {
        var jiraOptions = serviceProvider.GetRequiredService<IOptions<JiraOptions>>().Value;
        client.BaseAddress = new Uri($"{jiraOptions.Url}/rest/api/2/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jiraOptions.AccessToken);
      });

    return services;
  }
}