using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Contexts;

public class CalculoRiscoContextoTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };
        var data = DateTime.UtcNow;

        // Act
        var contexto = new CalculoRiscoContexto(investimentos, produtos, data);

        // Assert
        contexto.Investimentos.Should().BeSameAs(investimentos);
        contexto.Produtos.Should().BeSameAs(produtos);
        contexto.DataReferencia.Should().Be(data);
    }
}
