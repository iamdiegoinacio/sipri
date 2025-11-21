using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraCalculoCDBTests
{
    [Fact]
    public void TipoProduto_ShouldBeCDB()
    {
        var regra = new RegraCalculoCDB();
        regra.TipoProduto.Should().Be("CDB");
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenContextProductIsNull()
    {
        var regra = new RegraCalculoCDB();
        var contexto = new CalculoInvestimentoContexto(1000, 12, null!);

        Action act = () => regra.Calcular(contexto);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calcular_ShouldCalculateCorrectly_ForOneYear()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.10m }; // 10% ao ano
        var contexto = new CalculoInvestimentoContexto(1000m, 12, produto); // 1 ano

        // Act
        var result = regra.Calcular(contexto);

        // Assert
        // Juros = 1000 * 0.10 * 1 = 100
        // Final = 1100
        result.Should().Be(1100m);
    }

    [Fact]
    public void Calcular_ShouldCalculateCorrectly_ForSixMonths()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.10m }; // 10% ao ano
        var contexto = new CalculoInvestimentoContexto(1000m, 6, produto); // 0.5 ano

        // Act
        var result = regra.Calcular(contexto);

        // Assert
        // Juros = 1000 * 0.10 * 0.5 = 50
        // Final = 1050
        result.Should().Be(1050m);
    }
}
