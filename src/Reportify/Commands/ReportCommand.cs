using System.Reflection;
using Reportify.Configuration.Validation;
using Reportify.Report;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommand : AsyncCommand<ReportCommandSettings>
{
  private readonly IReportBuilder _reportBuilder;
  private readonly IReportWriter _reportWriter;
  private readonly IReportExporter _reportExporter;
  private readonly IConfigurationValidator _configurationValidator;
  private readonly IValidationErrorWriter _validationErrorWriter;

  public ReportCommand(IReportBuilder reportBuilder,
    IReportWriter reportWriter,
    IReportExporter reportExporter,
    IConfigurationValidator configurationValidator,
    IValidationErrorWriter validationErrorWriter)
  {
    _reportBuilder = reportBuilder;
    _reportWriter = reportWriter;
    _reportExporter = reportExporter;
    _configurationValidator = configurationValidator;
    _validationErrorWriter = validationErrorWriter;
  }

  public override async Task<int> ExecuteAsync(CommandContext commandContext, ReportCommandSettings settings)
  {
    var commandResult = settings.PrintVersion
      ? PrintVersion()
      : await ExecuteCommandAsync(settings);

    return (int)commandResult;
  }

  private async Task<CommandResult> ExecuteCommandAsync(ReportCommandSettings settings)
  {
    return await ConsoleProgress.StartAsync(
      async context =>
      {
        var validationErrors = await context.ExecuteTask(
          "Validating configuration...",
          () => _configurationValidator.ValidateAsync());

        if (validationErrors.Any())
        {
          _validationErrorWriter.Write(validationErrors);
          return CommandResult.Failure;
        }

        var report = await context.ExecuteTask(
          "Generating report...",
          () => BuildReportAsync(settings));

        _reportWriter.Write(report);

        if (settings.CopyToClipboard)
          _reportExporter.ExportToClipboard(report);

        return CommandResult.Success;
      });
  }

  private static CommandResult PrintVersion()
  {
    var version = Assembly.GetEntryAssembly().GetInformationalVersion();
    AnsiConsole.MarkupLine($"[bold]Reportify[/] version [cyan]{version}[/]");

    return CommandResult.Success;
  }

  private Task<Report.Report> BuildReportAsync(ReportCommandSettings settings)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);

    return settings.EntireWeek
      ? _reportBuilder.BuildAsync(today.GetFirstDayOfWeek(), today.GetLastDayOfWeek())
      : _reportBuilder.BuildAsync(settings.Date ?? today);
  }
}