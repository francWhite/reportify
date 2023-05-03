using Spectre.Console;

namespace Reportify.Report;

internal class CreateReportCommand : ICreateReportCommand
{
  public Task CreateForDateAsync(DateOnly date)
  {
    AnsiConsole.MarkupLine($"[green]Report for date:[/] {date}");
    return Task.CompletedTask;
  }

  public Task CreateForDateRangeAsync(DateOnly startDate, DateOnly endDate)
  {
    AnsiConsole.MarkupLine($"[green]Report for week, from:[/] {startDate} [green]to:[/] {endDate}");
    return Task.CompletedTask;
  }
}