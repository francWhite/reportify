using Spectre.Console;

namespace Reportify.Report;

internal class CreateReportCommand : ICreateReportCommand
{
  public Task CreateForDateAsync(DateOnly date)
  {
    AnsiConsole.MarkupLine($"[green]Report for date:[/] {date}");
    return Task.CompletedTask;
  }

  public Task CreateForWeekAsync()
  {
    var (startDate, endDate) = BuildCurrentWeekRange();

    AnsiConsole.MarkupLine($"[green]Report for current week, from:[/] {startDate} [green]to:[/] {endDate}");
    return Task.CompletedTask;
  }

  private static (DateOnly startDate, DateOnly endDate) BuildCurrentWeekRange()
  {
    var today = DateOnly.FromDateTime(DateTime.Today);
    var startDate = today.AddDays(-(int)today.DayOfWeek + 1);
    var endDate = startDate.AddDays(6);

    return (startDate, endDate);
  }
}