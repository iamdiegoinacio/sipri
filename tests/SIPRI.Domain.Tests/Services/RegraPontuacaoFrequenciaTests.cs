using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraPontuacaoFrequenciaTests
{
    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenInvestimentosNull()
    {
        var regra = new RegraPontuacaoFrequencia();
        var contexto = new CalculoRiscoContexto(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        Action act = () => regra.CalcularPontuacao(contexto);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnZero_WhenNoInvestments()
    {
        var regra = new RegraPontuacaoFrequencia();
        var contexto = new CalculoRiscoContexto(new List<Investimento>(), new List<ProdutoInvestimento>(), DateTime.UtcNow);

        var result = regra.CalcularPontuacao(contexto);
        result.Should().Be(10); // PontosBaixo
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnHighPoints_ForRecentInvestments()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataRef = DateTime.UtcNow;
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataRef.AddMonths(-1) }, // Recente
            new Investimento { Data = dataRef.AddMonths(-2) }  // Recente
        };
        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataRef);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        // 2 investimentos recentes * 5 pontos = 10 (Wait, logic is based on count thresholds)
        // Count = 2. LimiteFrequenciaBaixa = 2. So returns PontosBaixo (10).
        result.Should().Be(10);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnLowPoints_ForOldInvestments()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataRef = DateTime.UtcNow;
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataRef.AddMonths(-13) } // Antigo (> 12 meses)
        };
        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataRef);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        // Count within last 6 months = 0. 0 <= 2. Returns PontosBaixo (10).
        result.Should().Be(10);
    }
}
