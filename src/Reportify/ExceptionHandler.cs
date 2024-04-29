using System.Reflection;
using System.Runtime.InteropServices;
using Reportify.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reportify;

internal static class ExceptionHandler
{
  public static int Handle(Exception exception, ITypeResolver? typeResolver)
  {
    if (exception is CommandAppException { Pretty: not null } commandAppException)
    {
      AnsiConsole.Write(commandAppException.Pretty);
      return 1;
    }

    if (exception is CommandRuntimeException commandRuntimeException)
    {
      AnsiConsole.MarkupLine($"[red]Error:[/] {commandRuntimeException.Message}");
      return 1;
    }

    var file = CreateTempFile();
    WriteException(file, exception);

    AnsiConsole.MarkupLine($"An unexpected error occurred: [red]{exception.Message}[/]");
    AnsiConsole.MarkupLine("If the error persists, please report it at [blue]https://github.com/francWhite/reportify/issues[/]");
    AnsiConsole.MarkupLine($"Please include the following file in your report: [blue]{file.FullName}[/]");
    return 1;
  }

  private static FileInfo CreateTempFile()
  {
    var tempPath = Path.GetTempPath();
    var tempFile = $"reportify_{Guid.NewGuid()}.log";
    var tempFilePath = Path.Combine(tempPath, tempFile);
    return new FileInfo(tempFilePath);
  }

  private static void WriteException(FileInfo file, Exception e)
  {
    using var writer = file.CreateText();
    writer.WriteLine($"reportify {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
    writer.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
    writer.WriteLine($"timestamp: {DateTime.UtcNow:O}");
    writer.WriteLine();
    writer.WriteLine(e.ToString());
  }
}