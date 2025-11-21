using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraPontuacaoVolumeTests
{
    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenInvestimentosNull()
    {
        var regra = new RegraPontuacaoVolume();
        var contexto = new CalculoRiscoContexto(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        Action act = () => regra.CalcularPontuacao(contexto);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(5000, 5)]   // <= 5k (Baixo)
    [InlineData(10000, 10)]  // > 5k and <= 50k (Medio)
    [InlineData(10001, 10)] // <= 50k
    [InlineData(50000, 10)] // <= 50k
    [InlineData(50001, 20)] // > 50k
    public void CalcularPontuacao_ShouldReturnCorrectPoints_BasedOnTotalVolume(decimal totalValue, int expectedPoints)
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = totalValue }
        };
        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        result.Should().Be(expectedPoints);
    }

    [Fact]
    public void CalcularPontuacao_ShouldSumValuesFromMultipleInvestments()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 20000 },
            new Investimento { Valor = 35000 }
        };
        // Total = 55000 -> > 50k -> 20 pontos
        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        result.Should().Be(20);
    }
}
