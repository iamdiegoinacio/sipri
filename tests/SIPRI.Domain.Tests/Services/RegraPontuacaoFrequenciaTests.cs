using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraPontuacaoFrequenciaTests
{
    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenInvestimentosIsNull()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var contexto = new CalculoRiscoContexto(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        Action act = () => regra.CalcularPontuacao(contexto);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenDataReferenciaIsMinValue()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var contexto = new CalculoRiscoContexto(
            new List<Investimento>(), 
            new List<ProdutoInvestimento>(), 
            DateTime.MinValue);

        // Act
        Action act = () => regra.CalcularPontuacao(contexto);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Data de referência inválida*");
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnPontosBaixo_WhenFrequencyIsLow()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataReferencia = new DateTime(2025, 11, 21);
        
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataReferencia.AddMonths(-3) }, // Dentro dos 6 meses
            new Investimento { Data = dataReferencia.AddMonths(-5) }  // Dentro dos 6 meses
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataReferencia);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // 2 transações <= 2 -> PontosBaixo (10)
        pontuacao.Should().Be(10);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnPontosMedio_WhenFrequencyIsModerate()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataReferencia = new DateTime(2025, 11, 21);
        
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataReferencia.AddMonths(-1) },
            new Investimento { Data = dataReferencia.AddMonths(-2) },
            new Investimento { Data = dataReferencia.AddMonths(-3) },
            new Investimento { Data = dataReferencia.AddMonths(-4) }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataReferencia);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // 4 transações, 3 <= 4 <= 6 -> PontosMedio (25)
        pontuacao.Should().Be(25);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnPontosAlto_WhenFrequencyIsHigh()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataReferencia = new DateTime(2025, 11, 21);
        
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataReferencia.AddMonths(-1) },
            new Investimento { Data = dataReferencia.AddMonths(-1) },
            new Investimento { Data = dataReferencia.AddMonths(-2) },
            new Investimento { Data = dataReferencia.AddMonths(-3) },
            new Investimento { Data = dataReferencia.AddMonths(-4) },
            new Investimento { Data = dataReferencia.AddMonths(-5) },
            new Investimento { Data = dataReferencia.AddMonths(-5) }
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataReferencia);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // 7 transações > 6 -> PontosAlto (40)
        pontuacao.Should().Be(40);
    }

    [Fact]
    public void CalcularPontuacao_ShouldIgnoreTransactions_OutsideTimeWindow()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataReferencia = new DateTime(2025, 11, 21);
        
        var investimentos = new List<Investimento>
        {
            new Investimento { Data = dataReferencia.AddMonths(-1) },  // Dentro
            new Investimento { Data = dataReferencia.AddMonths(-7) },  // Fora (> 6 meses)
            new Investimento { Data = dataReferencia.AddMonths(-12) }  // Fora
        };

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataReferencia);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        // Apenas 1 transação dentro dos últimos 6 meses -> PontosBaixo (10)
        pontuacao.Should().Be(10);
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnPontosBaixo_WhenNoInvestments()
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var contexto = new CalculoRiscoContexto(
            new List<Investimento>(), 
            new List<ProdutoInvestimento>(), 
            DateTime.UtcNow);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        pontuacao.Should().Be(10);
    }

    [Theory]
    [InlineData(0, 10)]  // 0 transações -> Baixo
    [InlineData(1, 10)]  // 1 transação -> Baixo
    [InlineData(2, 10)]  // 2 transações -> Baixo (limite)
    [InlineData(3, 25)]  // 3 transações -> Médio
    [InlineData(6, 25)]  // 6 transações -> Médio (limite)
    [InlineData(7, 40)]  // 7 transações -> Alto
    [InlineData(10, 40)] // 10 transações -> Alto
    public void CalcularPontuacao_ShouldReturnCorrectPoints_BasedOnTransactionCount(int numTransacoes, int expectedPontos)
    {
        // Arrange
        var regra = new RegraPontuacaoFrequencia();
        var dataReferencia = DateTime.UtcNow;
        
        var investimentos = Enumerable.Range(0, numTransacoes)
            .Select(i => new Investimento { Data = dataReferencia.AddMonths(-1) })
            .ToList();

        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), dataReferencia);

        // Act
        var pontuacao = regra.CalcularPontuacao(contexto);

        // Assert
        pontuacao.Should().Be(expectedPontos);
    }
}
