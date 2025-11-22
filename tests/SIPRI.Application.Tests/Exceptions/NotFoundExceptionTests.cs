using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateExceptionWithEmptyMessage()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldCreateExceptionWithCustomMessage()
    {
        // Arrange
        var message = "O recurso solicitado não foi encontrado.";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateExceptionWithBoth()
    {
        // Arrange
        var message = "Erro ao buscar o recurso.";
        var innerException = new InvalidOperationException("Operação inválida");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Message.Should().Be("Operação inválida");
    }

    [Fact]
    public void Constructor_WithNameAndKey_ShouldCreateFormattedMessage()
    {
        // Arrange
        var name = "Cliente";
        var key = Guid.NewGuid();

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entidade \"{name}\" ({key}) não foi encontrada.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNameAndIntegerKey_ShouldCreateFormattedMessage()
    {
        // Arrange
        var name = "Produto";
        var key = 12345;

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entidade \"{name}\" ({key}) não foi encontrada.");
    }

    [Fact]
    public void Constructor_WithNameAndStringKey_ShouldCreateFormattedMessage()
    {
        // Arrange
        var name = "Investimento";
        var key = "INV-001";

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entidade \"{name}\" ({key}) não foi encontrada.");
    }

    [Fact]
    public void Constructor_WithEmptyMessage_ShouldCreateExceptionWithEmptyMessage()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithNameAndNullKey_ShouldCreateFormattedMessageWithNullKey()
    {
        // Arrange
        var name = "Simulacao";
        object key = null!;

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entidade \"{name}\" () não foi encontrada.");
    }

    [Fact]
    public void Constructor_WithEmptyNameAndKey_ShouldCreateFormattedMessage()
    {
        // Arrange
        var name = string.Empty;
        var key = 123;

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entidade \"\" ({key}) não foi encontrada.");
    }

    [Fact]
    public void Constructor_WithNullInnerException_ShouldCreateExceptionWithNullInner()
    {
        // Arrange
        var message = "Recurso não encontrado";
        Exception innerException = null!;

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }
}