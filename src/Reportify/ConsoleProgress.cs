using Spectre.Console;

namespace Reportify;

internal static class ConsoleProgress
{
  public static Task StartAsync(Func<ProgressContext, Task> action)
  {
    return AnsiConsole
      .Progress()
      .AutoClear(true)
      .Columns(new TaskDescriptionColumn(), new SpinnerColumn())
      .StartAsync(action);
  }
}