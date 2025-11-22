using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class ForbiddenAccessExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateExceptionWithDefaultMessage()
    {
        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("O usuário não tem permissão para executar esta ação.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithCustomMessage_ShouldCreateExceptionWithCustomMessage()
    {
        // Arrange
        var message = "Acesso negado ao recurso 'Investimento #123'.";

        // Act
        var exception = new ForbiddenAccessException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmptyMessage_ShouldCreateExceptionWithEmptyMessage()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var exception = new ForbiddenAccessException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new ForbiddenAccessException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DefaultConstructor_WithClientScenario_ShouldProvideDefaultMessage()
    {
        // Arrange
        // Cenário: Cliente "123" tentando acessar investimento de outro cliente

        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        exception.Message.Should().Be("O usuário não tem permissão para executar esta ação.");
    }

    [Fact]
    public void Constructor_WithResourceSpecificMessage_ShouldContainResourceInfo()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentoId = Guid.NewGuid();
        var message = $"Cliente '{clienteId}' não tem permissão para acessar o investimento '{investimentoId}'.";

        // Act
        var exception = new ForbiddenAccessException(message);

        // Assert
        exception.Message.Should().Contain(clienteId.ToString());
        exception.Message.Should().Contain(investimentoId.ToString());
        exception.Message.Should().Contain("não tem permissão");
    }

    [Fact]
    public void Constructor_WithRoleBasedMessage_ShouldContainRoleInfo()
    {
        // Arrange
        var message = "O usuário com perfil 'Cliente' não pode acessar recursos administrativos.";

        // Act
        var exception = new ForbiddenAccessException(message);

        // Assert
        exception.Message.Should().Contain("perfil 'Cliente'");
        exception.Message.Should().Contain("não pode acessar");
    }

    [Fact]
    public void Constructor_WithActionBasedMessage_ShouldContainActionInfo()
    {
        // Arrange
        var message = "O usuário não tem permissão para deletar simulações.";

        // Act
        var exception = new ForbiddenAccessException(message);

        // Assert
        exception.Message.Should().Contain("deletar simulações");
        exception.Message.Should().Contain("não tem permissão");
    }

    [Fact]
    public void DefaultMessage_ShouldBeInPortuguese()
    {
        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        exception.Message.Should().Be("O usuário não tem permissão para executar esta ação.");
    }

    [Fact]
    public void Constructor_WithLongMessage_ShouldPreserveFullMessage()
    {
        // Arrange
        var longMessage = "O usuário autenticado com ID '12345' e perfil 'Cliente' tentou acessar " +
                         "o recurso '/api/investimentos/admin/relatorios' que requer permissão de " +
                         "'Administrador'. Esta ação foi negada por política de segurança.";

        // Act
        var exception = new ForbiddenAccessException(longMessage);

        // Assert
        exception.Message.Should().Be(longMessage);
        exception.Message.Length.Should().BeGreaterThan(100);
    }
}