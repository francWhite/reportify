using Spectre.Console;

namespace Reportify;

internal static class ProgressContextExtensions
{
  public static async Task<T> ExecuteTask<T>(this ProgressContext progressContext, string description,
    Func<Task<T>> action)
  {
    var task = progressContext.AddTask(description);
    task.StartTask();

    var result = await action();
    task.StopTask();

    return result;
  }
}