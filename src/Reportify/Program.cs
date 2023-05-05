using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reportify.Commands;
using Reportify.Configuration;
using Reportify.Infrastructure;
using Reportify.Report;
using Spectre.Console.Cli;

var config = new ConfigurationBuilder()
  .AddJsonFile("reportify.config.json")
  .Build();

var services = new ServiceCollection()
  .Configure<ReportifyOptions>(config)
  .AddSingleton<IReportBuilder, ReportBuilder>()
  .AddSingleton<IReportWriter, ReportWriter>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ReportCommand>(registrar);

return await app.RunAsync(args);