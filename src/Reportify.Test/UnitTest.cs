using FluentAssertions;

namespace Reportify.Test;

public class UnitTest
{
  [Fact]
  public void Test()
  {
    true.Should().BeTrue();
  }
}