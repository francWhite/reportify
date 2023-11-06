using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reportify;
using Reportify.Commands;
using Reportify.Configuration;
using Reportify.Configuration.Validation;
using Reportify.Extensions;
using Reportify.Infrastructure;
using Reportify.Report;
using Reportify.Report.Jira;
using Reportify.Report.ManicTime;
using Reportify.Report.Output;
using Spectre.Console.Cli;

var config = new ConfigurationBuilder()
  .AddReportifyConfiguration(ConfigurationFileInfo.FilePath)
  .Build();

var services = new ServiceCollection()
  .Configure<JiraOptions>(config.GetSection("Jira"))
  .Configure<ManicTimeOptions>(config.GetSection("ManicTime"))
  .AddSingleton<IReportBuilder, ReportBuilder>()
  .AddSingleton<IReportWriter, ReportWriter>()
  .AddSingleton<IReportExporter, ReportExporter>()
  .AddSingleton<IActivityQuery, ActivityQuery>()
  .AddSingleton<IErpPositionEvaluator, ErpPositionEvaluator>()
  .AddSingleton<IJiraService, JiraService>()
  .AddSingleton<IConfigurationValidator, ConfigurationValidator>()
  .AddSingleton<IValidationErrorWriter, ValidationErrorWriter>()
  .AddSingleton<IOutputDataConverter, OutputDataConverter>()
  .AddJiraHttpClient();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ReportCommand>(registrar);
app.Configure(c => c.Settings.ExceptionHandler = ExceptionHandler.Handle);

return await app.RunAsync(args);