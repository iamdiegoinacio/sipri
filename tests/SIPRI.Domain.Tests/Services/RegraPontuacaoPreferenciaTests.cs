using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraPontuacaoPreferenciaTests
{
    [Fact]
    public void CalcularPontuacao_ShouldThrowException_WhenInvestimentosNull()
    {
        var regra = new RegraPontuacaoPreferencia();
        var contexto = new CalculoRiscoContexto(null!, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        Action act = () => regra.CalcularPontuacao(contexto);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularPontuacao_ShouldReturnPoints_BasedOnProductRisk()
    {
        // Arrange
        var regra = new RegraPontuacaoPreferencia();
        
        var prodBaixo = new ProdutoInvestimento { Id = Guid.NewGuid(), NivelRisco = 1 };
        var prodAlto = new ProdutoInvestimento { Id = Guid.NewGuid(), NivelRisco = 3 };
        
        var produtos = new List<ProdutoInvestimento> { prodBaixo, prodAlto };
        
        var investimentos = new List<Investimento>
        {
            new Investimento { ProdutoId = prodBaixo.Id, Valor = 1000 },
            new Investimento { ProdutoId = prodAlto.Id, Valor = 1000 }
        };

        var contexto = new CalculoRiscoContexto(investimentos, produtos, DateTime.UtcNow);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        // Total = 2000.
        // Peso Baixo (1) = 0.5. Peso Alto (3) = 0.5.
        // RMP = 0.5*1 + 0.5*3 = 0.5 + 1.5 = 2.0.
        // 1.5 <= 2.0 < 2.5 -> PontosMedio (25).
        result.Should().Be(25);
    }

    [Fact]
    public void CalcularPontuacao_ShouldIgnoreInvestments_WithUnknownProduct()
    {
        // Arrange
        var regra = new RegraPontuacaoPreferencia();
        var investimentos = new List<Investimento>
        {
            new Investimento { ProdutoId = Guid.NewGuid() } // ID não existe na lista de produtos
        };
        var contexto = new CalculoRiscoContexto(investimentos, new List<ProdutoInvestimento>(), DateTime.UtcNow);

        // Act
        var result = regra.CalcularPontuacao(contexto);

        // Assert
        // Assert
        result.Should().Be(10); // PontosBaixo
    }
}
