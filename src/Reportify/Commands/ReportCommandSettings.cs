using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommandSettings : CommandSettings
{
  [Description("Prints version information")]
  [CommandOption("-v|--version")]
  public bool PrintVersion { get; init; }

  [Description("Create report for a specified date, defaults to the current day")]
  [CommandOption("-d|--date <DATE>")]
  [TypeConverter(typeof(DateOnlyCurrentCultureConverter))]
  public DateOnly? Date { get; init; }

  [Description("Create report for the current week")]
  [CommandOption("-w|--week")]
  public bool EntireWeek { get; init; }

  [Description("Offset in weeks to determine the week used for the report")]
  [CommandOption("-o|--week-offset <OFFSET>")]
  [DefaultValue(0)]
  public int WeekOffset { get; init; }

  [Description("Copy report in CSV format to clipboard")]
  [CommandOption("-c|--copy")]
  public bool CopyToClipboard { get; init; }

  public override ValidationResult Validate()
  {
    if (Date is not null && EntireWeek) return ValidationResult.Error("date and week options are mutually exclusive");
    if (WeekOffset != 0 && !EntireWeek)
      return ValidationResult.Error("week offset can only be used with the week option");
    return ValidationResult.Success();
  }
}