using Xunit;
using FluentAssertions;
using DispenserProvider.Extensions;

namespace DispenserProvider.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void SpecifyUtcKind_ShouldReturnUtcKind()
    {
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        var result = date.SpecifyUtcKind();

        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void SpecifyUtcKind_WhenNull_ReturnsNull()
    {
        DateTime? date = null;

        var result = date.SpecifyUtcKind();

        result.Should().BeNull();
    }
}