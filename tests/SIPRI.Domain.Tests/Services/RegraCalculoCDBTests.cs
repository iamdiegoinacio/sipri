using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraCalculoCDBTests
{
    [Fact]
    public void TipoProduto_ShouldReturnCDB()
    {
        // Arrange
        var regra = new RegraCalculoCDB();

        // Act
        var tipo = regra.TipoProduto;

        // Assert
        tipo.Should().Be("CDB");
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenProdutoIsNull()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var contexto = new CalculoInvestimentoContexto(1000m, 12, null!);

        // Act
        Action act = () => regra.Calcular(contexto);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(1000, 12, 0.12, 1120.00)]
    [InlineData(5000, 6, 0.10, 5250.00)]
    [InlineData(10000, 24, 0.15, 13000.00)]
    public void Calcular_ShouldReturnCorrectValue_ForSimpleInterest(
        decimal valorInvestido, 
        int prazoMeses, 
        decimal rentabilidade, 
        decimal expectedValorFinal)
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Tipo = "CDB",
            RentabilidadeBase = rentabilidade
        };
        var contexto = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(expectedValorFinal);
    }

    [Theory]
    [InlineData(1000, 1, 0.12, 1010.00)] // 1 mês = 1/12 ano
    [InlineData(1000, 3, 0.12, 1030.00)] // 3 meses = 1/4 ano
    [InlineData(1000, 18, 0.12, 1180.00)] // 18 meses = 1.5 anos
    public void Calcular_ShouldHandleDifferentTimeframes(
        decimal valorInvestido, 
        int prazoMeses, 
        decimal rentabilidade, 
        decimal expectedValorFinal)
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento { RentabilidadeBase = rentabilidade };
        var contexto = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(expectedValorFinal);
    }

    [Fact]
    public void Calcular_ShouldHandleZeroInvestment()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.12m };
        var contexto = new CalculoInvestimentoContexto(0m, 12, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(0m);
    }

    [Fact]
    public void Calcular_ShouldHandleZeroRentabilidade()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0m };
        var contexto = new CalculoInvestimentoContexto(1000m, 12, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(1000m); // Sem juros, retorna o valor investido
    }

    [Fact]
    public void Calcular_ShouldApplySimpleInterestFormula()
    {
        // Arrange
        var regra = new RegraCalculoCDB();
        var valorInvestido = 10000m;
        var prazoMeses = 24;
        var rentabilidade = 0.10m;
        var produto = new ProdutoInvestimento { RentabilidadeBase = rentabilidade };
        var contexto = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        // Fórmula: Juros = ValorInvestido * Rentabilidade * (PrazoMeses / 12)
        // Juros = 10000 * 0.10 * 2 = 2000
        // ValorFinal = 10000 + 2000 = 12000
        resultado.Should().Be(12000m);
    }
}
