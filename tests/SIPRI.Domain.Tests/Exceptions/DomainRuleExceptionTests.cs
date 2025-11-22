using FluentAssertions;
using SIPRI.Domain.Exceptions;

namespace SIPRI.Domain.Tests.Exceptions;

public class DomainRuleExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessageProperty()
    {
        // Arrange
        var message = "Regra de negócio violada: valor inválido";

        // Act
        var exception = new DomainRuleException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBothProperties()
    {
        // Arrange
        var message = "Erro na validação do domínio";
        var innerException = new InvalidOperationException("Operação inválida");

        // Act
        var exception = new DomainRuleException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void DomainRuleException_ShouldBeThrowable()
    {
        // Arrange
        var message = "Tentativa de criar PerfilRisco com pontuação negativa";

        // Act
        Action act = () => throw new DomainRuleException(message);

        // Assert
        act.Should().Throw<DomainRuleException>()
            .WithMessage(message);
    }

    [Fact]
    public void DomainRuleException_ShouldBeCatchableAsException()
    {
        // Arrange
        var message = "Simulação com valor zero não é permitida";

        // Act
        Action act = () => throw new DomainRuleException(message);

        // Assert
        act.Should().Throw<Exception>()
            .Which.Should().BeOfType<DomainRuleException>();
    }

    [Theory]
    [InlineData("Valor de investimento deve ser maior que zero")]
    [InlineData("Prazo deve ser positivo")]
    [InlineData("Produto não encontrado")]
    public void Constructor_ShouldAcceptDifferentMessages(string message)
    {
        // Act
        var exception = new DomainRuleException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainRuleException_ShouldPreserveStackTrace()
    {
        // Arrange & Act
        try
        {
            throw new DomainRuleException("Teste de stack trace");
        }
        catch (DomainRuleException ex)
        {
            // Assert
            ex.StackTrace.Should().NotBeNullOrEmpty();
            ex.StackTrace.Should().Contain(nameof(DomainRuleExceptionTests));
        }
    }
}
