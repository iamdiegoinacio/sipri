using FluentAssertions;
using SIPRI.Domain.Exceptions;

namespace SIPRI.Domain.Tests.Exceptions;

public class DomainRuleExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        // Arrange
        var message = "Erro de regra de negócio";

        // Act
        var exception = new DomainRuleException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_ShouldSetMessageAndInnerException()
    {
        // Arrange
        var message = "Erro de regra de negócio";
        var inner = new Exception("Erro interno");

        // Act
        var exception = new DomainRuleException(message, inner);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(inner);
    }
}
