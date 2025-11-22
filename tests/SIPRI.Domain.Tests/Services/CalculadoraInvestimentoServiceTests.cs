using FluentAssertions;
using Moq;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class CalculadoraInvestimentoServiceTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenRulesAreNull()
    {
        // Act
        Action act = () => new CalculadoraInvestimentoService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenRulesAreEmpty()
    {
        // Act
        Action act = () => new CalculadoraInvestimentoService(Enumerable.Empty<IRegraCalculoInvestimento>());

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Nenhuma regra de cálculo foi injetada.*");
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenContextProductIsNull()
    {
        // Arrange
        var mockRule = new Mock<IRegraCalculoInvestimento>();
        mockRule.Setup(r => r.TipoProduto).Returns("CDB");
        var service = new CalculadoraInvestimentoService(new[] { mockRule.Object });
        var contexto = new CalculoInvestimentoContexto(1000, 12, null!);

        // Act
        Action act = () => service.Calcular(contexto);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenRuleNotFound()
    {
        // Arrange
        var mockRule = new Mock<IRegraCalculoInvestimento>();
        mockRule.Setup(r => r.TipoProduto).Returns("CDB");
        var service = new CalculadoraInvestimentoService(new[] { mockRule.Object });
        
        var produto = new ProdutoInvestimento { Tipo = "Fundo" };
        var contexto = new CalculoInvestimentoContexto(1000, 12, produto);

        // Act
        Action act = () => service.Calcular(contexto);

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage($"O tipo de produto '{produto.Tipo}' não possui uma regra de cálculo definida.");
    }

    [Fact]
    public void Calcular_ShouldExecuteCorrectRule_WhenRuleExists()
    {
        // Arrange
        var expectedValue = 1100m;
        var mockRule = new Mock<IRegraCalculoInvestimento>();
        mockRule.Setup(r => r.TipoProduto).Returns("CDB");
        mockRule.Setup(r => r.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(expectedValue);

        var service = new CalculadoraInvestimentoService(new[] { mockRule.Object });
        
        var produto = new ProdutoInvestimento { Tipo = "CDB" }; // Case insensitive check
        var contexto = new CalculoInvestimentoContexto(1000, 12, produto);

        // Act
        var result = service.Calcular(contexto);

        // Assert
        result.Should().Be(expectedValue);
        mockRule.Verify(r => r.Calcular(contexto), Times.Once);
    }

    [Fact]
    public void Calcular_ShouldExecuteCorrectRule_IgnoreCase()
    {
        // Arrange
        var expectedValue = 1100m;
        var mockRule = new Mock<IRegraCalculoInvestimento>();
        mockRule.Setup(r => r.TipoProduto).Returns("CDB");
        mockRule.Setup(r => r.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(expectedValue);

        var service = new CalculadoraInvestimentoService(new[] { mockRule.Object });
        
        var produto = new ProdutoInvestimento { Tipo = "cdb" }; // Lowercase
        var contexto = new CalculoInvestimentoContexto(1000, 12, produto);

        // Act
        var result = service.Calcular(contexto);

        // Assert
        result.Should().Be(expectedValue);
    }
}
