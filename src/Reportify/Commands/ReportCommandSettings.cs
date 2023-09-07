using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify.Commands;

internal class ReportCommandSettings : CommandSettings
{
  [Description("Prints version information")]
  [CommandOption("-v|--version")]
  public bool PrintVersion { get; init; }

  [Description("Create report for a specified date")]
  [CommandOption("-d|--date <DATE>")]
  [TypeConverter(typeof(DateOnlyCurrentCultureConverter))]
  public DateOnly? Date { get; init; }

  [Description("Create report for the current week")]
  [CommandOption("-w|--week")]
  public bool EntireWeek { get; init; }

  [Description("Copy report in CSV format to clipboard")]
  [CommandOption("-c|--copy")]
  public bool CopyToClipboard { get; init; }

  public override ValidationResult Validate()
  {
    return Date is not null && EntireWeek
      ? ValidationResult.Error("date and week options are mutually exclusive")
      : ValidationResult.Success();
  }
}