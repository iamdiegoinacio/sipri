using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class ConflictExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldCreateExceptionWithMessage()
    {
        // Arrange
        var message = "Os dados que você está tentando salvar estão desatualizados.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateExceptionWithBoth()
    {
        // Arrange
        var message = "Falha de concorrência otimista detectada.";
        var innerException = new InvalidOperationException("O registro foi modificado por outro usuário.");

        // Act
        var exception = new ConflictException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException!.Message.Should().Be("O registro foi modificado por outro usuário.");
    }

    [Fact]
    public void Constructor_WithConcurrencyMessage_ShouldContainConcurrencyInfo()
    {
        // Arrange
        var message = "A simulação foi modificada por outro usuário. Por favor, recarregue os dados e tente novamente.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Contain("modificada por outro usuário");
        exception.Message.Should().Contain("recarregue os dados");
    }

    [Fact]
    public void Constructor_WithDuplicateKeyMessage_ShouldContainDuplicateInfo()
    {
        // Arrange
        var message = "Já existe um investimento com o mesmo identificador.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Contain("Já existe");
        exception.Message.Should().Contain("mesmo identificador");
    }

    [Fact]
    public void Constructor_WithEmptyMessage_ShouldCreateExceptionWithEmptyMessage()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange
        var message = "Conflito detectado";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithNullInnerException_ShouldCreateExceptionWithNullInner()
    {
        // Arrange
        var message = "Conflito de estado do recurso";
        Exception innerException = null!;

        // Act
        var exception = new ConflictException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithDbUpdateConcurrencyException_ShouldPreserveInnerException()
    {
        // Arrange
        var message = "Erro de concorrência ao atualizar o registro.";
        var innerException = new InvalidOperationException("Database concurrency violation");

        // Act
        var exception = new ConflictException(message, innerException);

        // Assert
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
        exception.InnerException!.Message.Should().Contain("concurrency");
    }

    [Fact]
    public void Constructor_WithVersionConflictMessage_ShouldContainVersionInfo()
    {
        // Arrange
        var message = "A versão do registro (v3) não corresponde à versão esperada (v2). O registro foi modificado.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Contain("versão");
        exception.Message.Should().Contain("v3");
        exception.Message.Should().Contain("v2");
    }

    [Fact]
    public void Constructor_WithBusinessRuleConflictMessage_ShouldContainBusinessInfo()
    {
        // Arrange
        var message = "Não é possível criar uma nova simulação porque já existe uma simulação ativa para este produto.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Contain("já existe");
        exception.Message.Should().Contain("simulação ativa");
    }

    [Fact]
    public void Constructor_WithLongMessage_ShouldPreserveFullMessage()
    {
        // Arrange
        var longMessage = "Um conflito foi detectado ao tentar processar sua solicitação. " +
                         "O recurso que você está tentando modificar foi alterado por outro usuário " +
                         "desde que você o carregou. Para evitar perda de dados, recarregue o recurso " +
                         "e aplique suas alterações novamente.";

        // Act
        var exception = new ConflictException(longMessage);

        // Assert
        exception.Message.Should().Be(longMessage);
        exception.Message.Length.Should().BeGreaterThan(100);
    }

    [Fact]
    public void Constructor_WithEntitySpecificMessage_ShouldContainEntityInfo()
    {
        // Arrange
        var entityName = "Investimento";
        var entityId = Guid.NewGuid();
        var message = $"Conflito ao atualizar {entityName} com ID {entityId}. O registro foi modificado.";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Contain(entityName);
        exception.Message.Should().Contain(entityId.ToString());
        exception.Message.Should().Contain("Conflito");
    }
}