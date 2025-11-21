using FluentAssertions;
using Moq;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.Services;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Domain.Tests.Services;

public class MotorPerfilRiscoServiceTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenRulesAreNull()
    {
        Action act = () => new MotorPerfilRiscoServico(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenRulesAreEmpty()
    {
        Action act = () => new MotorPerfilRiscoServico(Enumerable.Empty<IRegraDePontuacao>());
        act.Should().Throw<ArgumentException>().WithMessage("*Nenhuma regra de pontuação foi injetada.*");
    }

    [Fact]
    public void CalcularPerfil_ShouldThrowException_WhenInvestimentosNull()
    {
        var mockRule = new Mock<IRegraDePontuacao>();
        var service = new MotorPerfilRiscoServico(new[] { mockRule.Object });

        Action act = () => service.CalcularPerfil(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPerfil_ShouldThrowException_WhenProdutosNull()
    {
        var mockRule = new Mock<IRegraDePontuacao>();
        var service = new MotorPerfilRiscoServico(new[] { mockRule.Object });

        Action act = () => service.CalcularPerfil(new List<Investimento>(), null!, DateTime.UtcNow);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPerfil_ShouldThrowException_WhenDataReferenciaIsMinValue()
    {
        var mockRule = new Mock<IRegraDePontuacao>();
        var service = new MotorPerfilRiscoServico(new[] { mockRule.Object });

        Action act = () => service.CalcularPerfil(new List<Investimento>(), new List<ProdutoInvestimento>(), DateTime.MinValue);
        act.Should().Throw<ArgumentException>().WithMessage("*A data de referência não pode ser MinValue.*");
    }

    [Fact]
    public void CalcularPerfil_ShouldReturnConservadorPadrao_WhenListsAreEmpty()
    {
        var mockRule = new Mock<IRegraDePontuacao>();
        var service = new MotorPerfilRiscoServico(new[] { mockRule.Object });

        var result = service.CalcularPerfil(new List<Investimento>(), new List<ProdutoInvestimento>(), DateTime.UtcNow);

        result.Should().BeEquivalentTo(PerfilRisco.ConservadorPadrao);
    }

    [Fact]
    public void CalcularPerfil_ShouldSumScoresFromAllRules()
    {
        // Arrange
        var mockRule1 = new Mock<IRegraDePontuacao>();
        mockRule1.Setup(r => r.CalcularPontuacao(It.IsAny<CalculoRiscoContexto>())).Returns(10);

        var mockRule2 = new Mock<IRegraDePontuacao>();
        mockRule2.Setup(r => r.CalcularPontuacao(It.IsAny<CalculoRiscoContexto>())).Returns(20);

        var service = new MotorPerfilRiscoServico(new[] { mockRule1.Object, mockRule2.Object });

        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };

        // Act
        var result = service.CalcularPerfil(investimentos, produtos, DateTime.UtcNow);

        // Assert
        // Total score = 30 -> Conservador (<= 35)
        result.Pontuacao.Should().Be(30);
        result.Perfil.Should().Be("Conservador");
    }

    [Fact]
    public void CalcularPerfil_ShouldReturnAgressivo_WhenScoreIsHigh()
    {
        // Arrange
        var mockRule1 = new Mock<IRegraDePontuacao>();
        mockRule1.Setup(r => r.CalcularPontuacao(It.IsAny<CalculoRiscoContexto>())).Returns(80);

        var service = new MotorPerfilRiscoServico(new[] { mockRule1.Object });

        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };

        // Act
        var result = service.CalcularPerfil(investimentos, produtos, DateTime.UtcNow);

        // Assert
        // Total score = 80 -> Agressivo (> 70)
        result.Pontuacao.Should().Be(80);
        result.Perfil.Should().Be("Agressivo");
    }
}
