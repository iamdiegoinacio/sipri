using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithSingleError_ShouldCreateExceptionWithOneError()
    {
        // Arrange
        var field = "TipoProduto";
        var message = "O tipo de produto é obrigatório.";

        // Act
        var exception = new ValidationException(field, message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("A requisição falhou na validação.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().HaveCount(1);
        exception.Errors.Should().ContainKey(field);
        exception.Errors[field].Should().HaveCount(1);
        exception.Errors[field][0].Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMultipleErrors_ShouldCreateExceptionWithAllErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "TipoProduto", new[] { "O tipo de produto é obrigatório.", "O tipo de produto deve ser válido." } },
            { "Valor", new[] { "O valor deve ser maior que zero." } },
            { "PrazoMeses", new[] { "O prazo deve ser entre 1 e 60 meses." } }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("A requisição falhou na validação.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().ContainKey("TipoProduto");
        exception.Errors.Should().ContainKey("Valor");
        exception.Errors.Should().ContainKey("PrazoMeses");
        exception.Errors["TipoProduto"].Should().HaveCount(2);
        exception.Errors["Valor"].Should().HaveCount(1);
        exception.Errors["PrazoMeses"].Should().HaveCount(1);
    }

    [Fact]
    public void Constructor_WithEmptyField_ShouldCreateExceptionWithEmptyKey()
    {
        // Arrange
        var field = string.Empty;
        var message = "Erro genérico de validação.";

        // Act
        var exception = new ValidationException(field, message);

        // Assert
        exception.Should().NotBeNull();
        exception.Errors.Should().ContainKey(string.Empty);
        exception.Errors[string.Empty][0].Should().Be(message);
    }

    [Fact]
    public void Constructor_WithEmptyDictionary_ShouldCreateExceptionWithNoErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>();

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Errors_ShouldBeReadOnly()
    {
        // Arrange
        var field = "ClienteId";
        var message = "O ID do cliente é obrigatório.";
        var exception = new ValidationException(field, message);

        // Act & Assert
        exception.Errors.Should().BeAssignableTo<IReadOnlyDictionary<string, string[]>>();
    }

    [Fact]
    public void Constructor_WithNullMessage_ShouldCreateExceptionWithNullMessage()
    {
        // Arrange
        var field = "Campo";
        string message = null!;

        // Act
        var exception = new ValidationException(field, message);

        // Assert
        exception.Should().NotBeNull();
        exception.Errors[field][0].Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange
        var field = "Test";
        var message = "Test message";

        // Act
        var exception = new ValidationException(field, message);

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithMultipleErrorsForSameField_ShouldStoreAllMessages()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "O email é obrigatório.", "O email deve ter formato válido.", "O email já está em uso." } }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors["Email"].Should().HaveCount(3);
        exception.Errors["Email"][0].Should().Be("O email é obrigatório.");
        exception.Errors["Email"][1].Should().Be("O email deve ter formato válido.");
        exception.Errors["Email"][2].Should().Be("O email já está em uso.");
    }

    [Fact]
    public void Message_ShouldAlwaysReturnDefaultMessage()
    {
        // Arrange & Act
        var exception1 = new ValidationException("Field1", "Error1");
        var exception2 = new ValidationException(new Dictionary<string, string[]>
        {
            { "Field2", new[] { "Error2" } }
        });

        // Assert
        exception1.Message.Should().Be("A requisição falhou na validação.");
        exception2.Message.Should().Be("A requisição falhou na validação.");
    }
}