using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Services;

namespace SIPRI.Domain.Tests.Services;

public class RegraCalculoFundoTests
{
    [Fact]
    public void TipoProduto_ShouldReturnFundo()
    {
        // Arrange
        var regra = new RegraCalculoFundo();

        // Act
        var tipo = regra.TipoProduto;

        // Assert
        tipo.Should().Be("Fundo");
    }

    [Fact]
    public void Calcular_ShouldThrowException_WhenProdutoIsNull()
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var contexto = new CalculoInvestimentoContexto(1000m, 12, null!);

        // Act
        Action act = () => regra.Calcular(contexto);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(1000, 12, 0.15, 1150.00)]
    [InlineData(5000, 6, 0.12, 5300.00)]
    [InlineData(10000, 24, 0.18, 13600.00)]
    public void Calcular_ShouldReturnCorrectValue_ForSimpleInterest(
        decimal valorInvestido, 
        int prazoMeses, 
        decimal rentabilidade, 
        decimal expectedValorFinal)
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Tipo = "Fundo",
            RentabilidadeBase = rentabilidade
        };
        var contexto = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(expectedValorFinal);
    }

    [Fact]
    public void Calcular_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            RentabilidadeBase = 0.156789m
        };
        var contexto = new CalculoInvestimentoContexto(1000m, 12, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        // 1000 + (1000 * 0.156789 * 1) = 1156.789 -> arredondado para 1156.79
        resultado.Should().Be(1156.79m);
    }

    [Theory]
    [InlineData(1000, 1, 0.15, 1012.50)]
    [InlineData(1000, 3, 0.15, 1037.50)]
    [InlineData(1000, 18, 0.15, 1225.00)]
    public void Calcular_ShouldHandleDifferentTimeframes(
        decimal valorInvestido, 
        int prazoMeses, 
        decimal rentabilidade, 
        decimal expectedValorFinal)
    {
        // Arrange
        var regra = new RegraCalculoFundo();
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
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.15m };
        var contexto = new CalculoInvestimentoContexto(0m, 12, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(0m);
    }

    [Fact]
    public void Calcular_ShouldHandleHighRentability()
    {
        // Arrange
        var regra = new RegraCalculoFundo();
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0.50m }; // 50% ao ano
        var contexto = new CalculoInvestimentoContexto(1000m, 12, produto);

        // Act
        var resultado = regra.Calcular(contexto);

        // Assert
        resultado.Should().Be(1500m);
    }

    [Fact]
    public void Calcular_ShouldUseSameFormulaAsCDB()
    {
        // Arrange
        var regraFundo = new RegraCalculoFundo();
        var regraCDB = new RegraCalculoCDB();
        var valorInvestido = 5000m;
        var prazoMeses = 12;
        var rentabilidade = 0.12m;

        var produtoFundo = new ProdutoInvestimento { RentabilidadeBase = rentabilidade };
        var produtoCDB = new ProdutoInvestimento { RentabilidadeBase = rentabilidade };

        var contextoFundo = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produtoFundo);
        var contextoCDB = new CalculoInvestimentoContexto(valorInvestido, prazoMeses, produtoCDB);

        // Act
        var resultadoFundo = regraFundo.Calcular(contextoFundo);
        var resultadoCDB = regraCDB.Calcular(contextoCDB);

        // Assert
        // Ambas as regras usam juros simples, então devem retornar o mesmo valor
        resultadoFundo.Should().Be(resultadoCDB);
    }
}
