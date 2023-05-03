using Microsoft.Extensions.DependencyInjection;
using Reportify.Commands;
using Reportify.Infrastructure;
using Reportify.Report;
using Spectre.Console.Cli;

var services = new ServiceCollection()
  .AddSingleton<ICreateReportCommand, CreateReportCommand>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ReportCommand>(registrar);

return await app.RunAsync(args);