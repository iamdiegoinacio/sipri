using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Contexts;

public class CalculoInvestimentoContextoExtendedTests
{
    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000.50)]
    public void ValorInvestido_ShouldAcceptValidValues(decimal valor)
    {
        // Arrange
        var produto = new ProdutoInvestimento { Id = Guid.NewGuid() };

        // Act
        var contexto = new CalculoInvestimentoContexto(valor, 12, produto);

        // Assert
        contexto.ValorInvestido.Should().Be(valor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public void PrazoMeses_ShouldAcceptValidValues(int prazo)
    {
        // Arrange
        var produto = new ProdutoInvestimento { Id = Guid.NewGuid() };

        // Act
        var contexto = new CalculoInvestimentoContexto(1000, prazo, produto);

        // Assert
        contexto.PrazoMeses.Should().Be(prazo);
    }

    [Fact]
    public void Produto_ShouldStoreCompleteProductInformation()
    {
        // Arrange
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Teste",
            Tipo = "CDB",
            RentabilidadeBase = 0.12m,
            Risco = "Baixo",
            NivelRisco = 1
        };

        // Act
        var contexto = new CalculoInvestimentoContexto(1000, 12, produto);

        // Assert
        contexto.Produto.Should().BeSameAs(produto);
        contexto.Produto.Nome.Should().Be("CDB Teste");
        contexto.Produto.RentabilidadeBase.Should().Be(0.12m);
    }

    [Fact]
    public void Constructor_ShouldAllowZeroValues()
    {
        // Arrange
        var produto = new ProdutoInvestimento { RentabilidadeBase = 0 };

        // Act
        var contexto = new CalculoInvestimentoContexto(0, 0, produto);

        // Assert
        contexto.ValorInvestido.Should().Be(0);
        contexto.PrazoMeses.Should().Be(0);
    }
}