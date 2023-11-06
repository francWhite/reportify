using System.Reflection;
using Reportify.Configuration.Validation;
using Reportify.Extensions;
using Reportify.Report;
using Reportify.Report.Output;
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
  private readonly IOutputDataConverter _outputDataConverter;

  public ReportCommand(IReportBuilder reportBuilder,
    IReportWriter reportWriter,
    IReportExporter reportExporter,
    IConfigurationValidator configurationValidator,
    IValidationErrorWriter validationErrorWriter,
    IOutputDataConverter outputDataConverter)
  {
    _reportBuilder = reportBuilder;
    _reportWriter = reportWriter;
    _reportExporter = reportExporter;
    _configurationValidator = configurationValidator;
    _validationErrorWriter = validationErrorWriter;
    _outputDataConverter = outputDataConverter;
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

        var outputData = _outputDataConverter.Convert(report);
        _reportWriter.Write(outputData);

        if (settings.CopyToClipboard)
          _reportExporter.ExportToClipboard(outputData);

        return CommandResult.Success;
      });
  }

  private static CommandResult PrintVersion()
  {
    var version = Assembly.GetEntryAssembly().GetInformationalVersion();
    AnsiConsole.MarkupLine($"[bold]reportify[/] version [cyan]{version}[/]");

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