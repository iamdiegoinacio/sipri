using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void ConflictException_ShouldSetMessage()
    {
        var ex = new ConflictException("Conflict occurred");
        ex.Message.Should().Be("Conflict occurred");
    }

    [Fact]
    public void ForbiddenAccessException_ShouldSetMessage()
    {
        var ex = new ForbiddenAccessException();
        ex.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InfrastructureException_ShouldSetMessageAndInnerException()
    {
        var inner = new Exception("Inner error");
        var ex = new InfrastructureException("TestService", "Infra error", inner);
        
        ex.Message.Should().Contain("TestService").And.Contain("Infra error");
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void NotFoundException_ShouldSetMessage_WithEntityAndKey()
    {
        var ex = new NotFoundException("EntityName", "Key123");
        ex.Message.Should().Contain("EntityName").And.Contain("Key123");
    }

    [Fact]
    public void NotFoundException_ShouldSetMessage_WithCustomMessage()
    {
        var ex = new NotFoundException("Custom message");
        ex.Message.Should().Be("Custom message");
    }

    [Fact]
    public void ValidationException_ShouldInitializeWithSingleError()
    {
        var ex = new ValidationException("Field1", "Error message");
        ex.Errors.Should().ContainKey("Field1");
        ex.Errors["Field1"].Should().Contain("Error message");
    }

    [Fact]
    public void ValidationException_ShouldInitializeWithFailures()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error 1" } }
        };
        var ex = new ValidationException(errors);
        ex.Errors.Should().BeEquivalentTo(errors);
    }
}
