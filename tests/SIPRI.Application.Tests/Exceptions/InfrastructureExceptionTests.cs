using FluentAssertions;
using SIPRI.Application.Exceptions;

namespace SIPRI.Application.Tests.Exceptions;

public class InfrastructureExceptionTests
{
    [Fact]
    public void Constructor_WithServiceNameAndMessage_ShouldCreateFormattedMessage()
    {
        // Arrange
        var serviceName = "BancoDeDados";
        var message = "Timeout ao executar a query.";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: {message}");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithServiceNameMessageAndInnerException_ShouldCreateExceptionWithAll()
    {
        // Arrange
        var serviceName = "ApiExterna";
        var message = "Falha na comunicação.";
        var innerException = new TimeoutException("A operação excedeu o tempo limite");

        // Act
        var exception = new InfrastructureException(serviceName, message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: {message}");
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Should().BeOfType<TimeoutException>();
        exception.InnerException!.Message.Should().Be("A operação excedeu o tempo limite");
    }

    [Fact]
    public void Constructor_WithDatabaseService_ShouldCreateDatabaseErrorMessage()
    {
        // Arrange
        var serviceName = "SQL Server";
        var message = "Conexão recusada.";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: {message}");
    }

    [Fact]
    public void Constructor_WithExternalApiService_ShouldCreateApiErrorMessage()
    {
        // Arrange
        var serviceName = "API Cotação";
        var message = "Serviço indisponível (503).";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: {message}");
    }

    [Fact]
    public void Constructor_WithEmptyServiceName_ShouldCreateMessageWithEmptyName()
    {
        // Arrange
        var serviceName = string.Empty;
        var message = "Erro genérico.";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '' falhou: {message}");
    }

    [Fact]
    public void Constructor_WithNullServiceName_ShouldCreateMessageWithNullName()
    {
        // Arrange
        string serviceName = null!;
        var message = "Erro genérico.";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '' falhou: {message}");
    }

    [Fact]
    public void Constructor_WithEmptyMessage_ShouldCreateMessageWithEmptyError()
    {
        // Arrange
        var serviceName = "Redis";
        var message = string.Empty;

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: ");
    }

    [Fact]
    public void Constructor_WithNullMessage_ShouldCreateMessageWithNullError()
    {
        // Arrange
        var serviceName = "Cache";
        string message = null!;

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: ");
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange
        var serviceName = "Serviço";
        var message = "Mensagem";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithNullInnerException_ShouldCreateExceptionWithNullInner()
    {
        // Arrange
        var serviceName = "Messaging Queue";
        var message = "Falha ao publicar mensagem.";
        Exception innerException = null!;

        // Act
        var exception = new InfrastructureException(serviceName, message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithHttpRequestException_ShouldPreserveInnerException()
    {
        // Arrange
        var serviceName = "REST API";
        var message = "Erro HTTP 500";
        var innerException = new HttpRequestException("Internal Server Error");

        // Act
        var exception = new InfrastructureException(serviceName, message, innerException);

        // Assert
        exception.InnerException.Should().BeOfType<HttpRequestException>();
        exception.InnerException!.Message.Should().Contain("Internal Server Error");
    }

    [Fact]
    public void Constructor_WithComplexServiceName_ShouldFormatCorrectly()
    {
        // Arrange
        var serviceName = "Serviço de Cotação B3 - v2.0";
        var message = "Rate limit excedido.";

        // Act
        var exception = new InfrastructureException(serviceName, message);

        // Assert
        exception.Message.Should().Be($"O serviço externo '{serviceName}' falhou: {message}");
    }
}