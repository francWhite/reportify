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
  .AddJsonFile("reportify.config.json")
  .Build();

var services = new ServiceCollection()
  .Configure<ReportifyOptions>(config)
  .AddSingleton<IReportBuilder, ReportBuilder>()
  .AddSingleton<IReportWriter, ReportWriter>()
  .AddSingleton<IActivityQuery, ActivityQuery>()
  .AddSingleton<IErpPositionEvaluator, ErpPositionEvaluator>()
  .AddSingleton<IJiraService, JiraService>();

services.AddHttpClient<JiraService>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ReportCommand>(registrar);

return await app.RunAsync(args);