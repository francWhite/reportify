using Spectre.Console;

namespace Reportify.Extensions;

internal static class ConsoleProgress
{
  public static Task<T> StartAsync<T>(Func<ProgressContext, Task<T>> action)
  {
    return AnsiConsole
      .Progress()
      .AutoClear(true)
      .Columns(new TaskDescriptionColumn(), new SpinnerColumn())
      .StartAsync(action);
  }
}