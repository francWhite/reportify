using Reportify.Report;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommand : AsyncCommand<ReportCommandSettings>
{
  private readonly IReportBuilder _reportBuilder;
  private readonly IReportWriter _reportWriter;
  private readonly IReportExporter _reportExporter;

  public ReportCommand(IReportBuilder reportBuilder, IReportWriter reportWriter, IReportExporter reportExporter)
  {
    _reportBuilder = reportBuilder;
    _reportWriter = reportWriter;
    _reportExporter = reportExporter;
  }

  public override async Task<int> ExecuteAsync(CommandContext commandContext, ReportCommandSettings settings)
  {
    await ConsoleProgress.StartAsync(
      async context =>
      {
        var task = context.AddTask("Generating report...").IsIndeterminate();
        task.StartTask();

        var report = await BuildReportAsync(settings);
        task.StopTask();

        _reportWriter.Write(report);

        if (settings.CopyToClipboard)
          _reportExporter.ExportToClipboard(report);
      });

    return 0;
  }

  private Task<Report.Report> BuildReportAsync(ReportCommandSettings settings)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);

    return settings.EntireWeek
      ? _reportBuilder.BuildAsync(today.GetFirstDayOfWeek(), today.GetLastDayOfWeek())
      : _reportBuilder.BuildAsync(settings.Date ?? today);
  }
}