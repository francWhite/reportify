using Reportify.Report;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommand : AsyncCommand<ReportCommandSettings>
{
  private readonly IReportBuilder _reportBuilder;
  private readonly IReportWriter _reportWriter;

  public ReportCommand(IReportBuilder reportBuilder, IReportWriter reportWriter)
  {
    _reportBuilder = reportBuilder;
    _reportWriter = reportWriter;
  }

  public override async Task<int> ExecuteAsync(CommandContext commandContext, ReportCommandSettings settings)
  {
    await AnsiConsole
      .Progress()
      .AutoClear(true)
      .Columns(new TaskDescriptionColumn(), new SpinnerColumn())
      .StartAsync(
        async context =>
        {
          var task = context.AddTask("Generating report...").IsIndeterminate();
          task.StartTask();

          var report = await BuildReportAsync(settings);
          task.StopTask();

          _reportWriter.Write(report);
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