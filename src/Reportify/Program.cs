using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reportify.Commands;
using Reportify.Configuration;
using Reportify.Infrastructure;
using Reportify.Report;
using Reportify.Report.Jira;
using Reportify.Report.ManicTime;
using Spectre.Console.Cli;

var config = new ConfigurationBuilder()
  .AddReportifyConfiguration()
  .Build();

var services = new ServiceCollection()
  .Configure<JiraOptions>(config.GetSection("Jira"))
  .Configure<ManicTimeOptions>(config.GetSection("ManicTime"))
  .AddSingleton<IReportBuilder, ReportBuilder>()
  .AddSingleton<IReportWriter, ReportWriter>()
  .AddSingleton<IActivityQuery, ActivityQuery>()
  .AddSingleton<IErpPositionEvaluator, ErpPositionEvaluator>()
  .AddSingleton<IJiraService, JiraService>()
  .AddSingleton<IConfigurationValidator, ConfigurationValidator>();

services.AddHttpClient<JiraService>();
services.AddHttpClient<ConfigurationValidator>();

var configurationValidator = services.BuildServiceProvider().GetRequiredService<IConfigurationValidator>();
await configurationValidator.ValidateAsync();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ReportCommand>(registrar);

return await app.RunAsync(args);