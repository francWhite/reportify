using Reportify.Report;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommand : AsyncCommand<ReportCommandSettings>
{
  private readonly ICreateReportCommand _createReportCommand;

  public ReportCommand(ICreateReportCommand createReportCommand)
  {
    _createReportCommand = createReportCommand;
  }

  public override async Task<int> ExecuteAsync(CommandContext context, ReportCommandSettings settings)
  {
    await CreateReportAsync(settings);
    return 0;
  }

  private Task CreateReportAsync(ReportCommandSettings settings)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);

    return settings.EntireWeek
      ? _createReportCommand.CreateForDateRangeAsync(today.GetFirstDayOfWeek(), today.GetLastDayOfWeek())
      : _createReportCommand.CreateForDateAsync(settings.Date ?? today);
  }
}