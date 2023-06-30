namespace Reportify.Configuration;

internal static class ConfigurationFileInfo
{
  public const string FileName = ".reportify";
  public static string FolderPath =>  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
  public static string FilePath =>  Path.Combine(FolderPath, FileName);
}