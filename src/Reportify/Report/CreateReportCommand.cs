using Microsoft.Extensions.Options;
using Reportify.Configuration;
using Spectre.Console;

namespace Reportify.Report;

internal class CreateReportCommand : ICreateReportCommand
{
  private readonly ReportifyOptions _reportifyOptions;

  public CreateReportCommand(IOptions<ReportifyOptions> reportifyOptions)
  {
    _reportifyOptions = reportifyOptions.Value;
  }

  public Task CreateForDateAsync(DateOnly date)
  {
    AnsiConsole.MarkupLine($"[green]Report for date:[/] {date}");
    AnsiConsole.MarkupLine($"[yellow]Jira Url:[/] {_reportifyOptions.JiraUrl}");
    return Task.CompletedTask;
  }

  public Task CreateForDateRangeAsync(DateOnly startDate, DateOnly endDate)
  {
    AnsiConsole.MarkupLine($"[green]Report for week, from:[/] {startDate} [green]to:[/] {endDate}");
    return Task.CompletedTask;
  }
}