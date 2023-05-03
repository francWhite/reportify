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
    return settings.EntireWeek
      ? _createReportCommand.CreateForWeekAsync()
      : _createReportCommand.CreateForDateAsync(settings.Date ?? DateOnly.FromDateTime(DateTime.Now));
  }
}