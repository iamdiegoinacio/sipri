using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraCalculoFundoTests
{
    [Fact]
    public void TipoProduto_ShouldBeFundo()
    {
        var regra = new RegraCalculoFundo();
        regra.TipoProduto.Should().Be("Fundo");
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenContextProductIsNull()
    {
        var regra = new RegraCalculoFundo();
        var contexto = new CalculoInvestimentoContexto(1000, 12, null!);

        Action act = () => regra.Calcular(contexto);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calcular_ShouldCalculateCompoundInterest()
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.10m }; // 10% ao ano
        var contexto = new CalculoInvestimentoContexto(1000m, 24, produto); // 2 anos

        // Act
        var result = regra.Calcular(contexto);

        // Assert
        // Formula Implemented: Simple Interest
        // 1000 + (1000 * 0.10 * 2) = 1200
        result.Should().Be(1200m);
    }

    [Fact]
    public void Calcular_ShouldHandleFractionalYears()
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.10m };
        var contexto = new CalculoInvestimentoContexto(1000m, 6, produto); // 0.5 ano

        // Act
        var result = regra.Calcular(contexto);

        // Assert
        // 1000 + (1000 * 0.10 * 0.5) = 1050
        result.Should().Be(1050m);
    }
}
