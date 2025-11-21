using FluentAssertions;
using SIPRI.Infrastructure.Services;

namespace SIPRI.Infrastructure.Tests.Services;

public class DateTimeProviderTests
{
    [Fact]
    public void UtcNow_ShouldReturnCurrentUtcTime()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var result = provider.UtcNow;

        // Assert
        result.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Today_ShouldReturnCurrentDateWithoutTime()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var result = provider.Today;

        // Assert
        result.Should().Be(DateTime.Today);
        result.TimeOfDay.Should().Be(TimeSpan.Zero);
    }
}
