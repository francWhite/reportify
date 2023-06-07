using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommandSettings : CommandSettings
{
  [Description("Create report for a specified date")]
  [CommandOption("-d|--date <DATE>")]
  [TypeConverter(typeof(DateOnlyCurrentCultureConverter))]
  public DateOnly? Date { get; init; }

  [Description("Create report for the current week")]
  [CommandOption("-w|--week")]
  public bool EntireWeek { get; init; }

  public override ValidationResult Validate()
  {
    return Date is not null && EntireWeek
      ? ValidationResult.Error("date and week options are mutually exclusive")
      : ValidationResult.Success();
  }
}