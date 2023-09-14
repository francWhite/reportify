using System.Reflection;

namespace Reportify.Extensions;

internal static class AssemblyExtensions
{
  public static string GetInformationalVersion(this Assembly? assembly)
  {
    return assembly
      ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
      ?.InformationalVersion ?? "dev";
  }
}