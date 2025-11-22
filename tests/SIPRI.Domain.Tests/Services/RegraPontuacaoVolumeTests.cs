using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraPontuacaoVolumeTests
{
    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenInvestimentosIsNull()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var contexto = new CalculoRiscoContexto(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        Action act = () => regra.CalcularPontuacao(contexto);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturn5Points_WhenVolumeIsLow()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 2000m },
            new Investimento { Valor = 2000m }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume total = 4000 <= 5000 -> 20 * 0.25 = 5
        pontuacao.Should().Be(5);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturn10Points_WhenVolumeIsModerate()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 15000m },
            new Investimento { Valor = 10000m }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume total = 25000, 5000 < 25000 <= 50000 -> 20 * 0.5 = 10
        pontuacao.Should().Be(10);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturn20Points_WhenVolumeIsHigh()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 30000m },
            new Investimento { Valor = 30000m }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume total = 60000 > 50000 -> 20
        pontuacao.Should().Be(20);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturn5Points_WhenNoInvestments()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var contexto = new CalculoRiscoContexto(
            new List<Investimento>(), 
            new List<ProdutoInvestimento>(), 
            DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume = 0 <= 5000 -> 5 pontos
        pontuacao.Should().Be(5);
    }

    [Theory]
    [InlineData(0, 5)]        // Volume = 0
    [InlineData(1000, 5)]     // Volume baixo
    [InlineData(5000, 5)]     // Limite baixo
    [InlineData(5001, 10)]    // Acima do limite baixo
    [InlineData(25000, 10)]   // Volume moderado
    [InlineData(50000, 10)]   // Limite moderado
    [InlineData(50001, 20)]   // Acima do limite moderado
    [InlineData(100000, 20)]  // Volume alto
    public void CalcularPontuacao_ShouldReturnCorrectPoints_BasedOnVolume(decimal volume, int expectedPontos)
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = volume }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        pontuacao.Should().Be(expectedPontos);
    }

    [Fact]
    public void CalcularPontuacao_ShouldSumAllInvestmentValues()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 10000m },
            new Investimento { Valor = 15000m },
            new Investimento { Valor = 20000m },
            new Investimento { Valor = 10000m }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume total = 55000 > 50000 -> 20 pontos
        pontuacao.Should().Be(20);
    }

    [Fact]
    public void CalcularPontuacao_ShouldHandleDecimalPrecision()
    {
        // Arrange
        var regra = new RegraPontuacaoVolume();
        var investimentos = new List<Investimento>
        {
            new Investimento { Valor = 2500.50m },
            new Investimento { Valor = 2499.50m }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Volume = 5000 <= 5000 -> 5 pontos
        pontuacao.Should().Be(5);
    }
}
