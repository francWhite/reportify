namespace Reportify.Tests;

internal class TempFile : IDisposable
{
  public TempFile Create(string? content = null)
  {
    File.WriteAllText(FilePath, content);
    return this;
  }

  public string FilePath { get; } = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");

  public void Dispose()
  {
    File.Delete(FilePath);
  }
}