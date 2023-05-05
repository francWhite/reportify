using Reportify.Report;
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

  public override async Task<int> ExecuteAsync(CommandContext context, ReportCommandSettings settings)
  {
    var report = await CreateReportAsync(settings);
    _reportWriter.Write(report);

    return 0;
  }

  private Task<Report.Report> CreateReportAsync(ReportCommandSettings settings)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);

    return settings.EntireWeek
      ? _reportBuilder.BuildAsync(today.GetFirstDayOfWeek(), today.GetLastDayOfWeek())
      : _reportBuilder.BuildAsync(settings.Date ?? today);
  }
}