using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Contexts;

public class CalculoRiscoContextoExtendedTests
{
    [Fact]
    public void Constructor_ShouldAcceptEmptyCollections()
    {
        // Arrange
        var investimentos = new List<Investimento>();
        var produtos = new List<ProdutoInvestimento>();
        var data = DateTime.UtcNow;

        // Act
        var contexto = new CalculoRiscoContexto(investimentos, produtos, data);

        // Assert
        contexto.Investimentos.Should().BeEmpty();
        contexto.Produtos.Should().BeEmpty();
        contexto.DataReferencia.Should().Be(data);
    }

    [Fact]
    public void Constructor_ShouldStoreMultipleInvestments()
    {
        // Arrange
        var investimentos = new List<Investimento>
        {
            new Investimento { Id = Guid.NewGuid(), Valor = 1000 },
            new Investimento { Id = Guid.NewGuid(), Valor = 2000 },
            new Investimento { Id = Guid.NewGuid(), Valor = 3000 }
        };
        var produtos = new List<ProdutoInvestimento>();
        var data = DateTime.UtcNow;

        // Act
        var contexto = new CalculoRiscoContexto(investimentos, produtos, data);

        // Assert
        contexto.Investimentos.Should().HaveCount(3);
        contexto.Investimentos.Should().BeSameAs(investimentos);
    }

    [Fact]
    public void Constructor_ShouldStoreMultipleProdutos()
    {
        // Arrange
        var investimentos = new List<Investimento>();
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Produto 1" },
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Produto 2" }
        };
        var data = DateTime.UtcNow;

        // Act
        var contexto = new CalculoRiscoContexto(investimentos, produtos, data);

        // Assert
        contexto.Produtos.Should().HaveCount(2);
        contexto.Produtos.Should().BeSameAs(produtos);
    }

    [Fact]
    public void DataReferencia_ShouldPreserveDateTime()
    {
        // Arrange
        var data = new DateTime(2025, 11, 21, 14, 30, 0, DateTimeKind.Utc);

        // Act
        var contexto = new CalculoRiscoContexto(
            new List<Investimento>(), 
            new List<ProdutoInvestimento>(), 
            data);

        // Assert
        contexto.DataReferencia.Should().Be(data);
        contexto.DataReferencia.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Constructor_ShouldNotModifyInputCollections()
    {
        // Arrange
        var investimentos = new List<Investimento>
        {
            new Investimento { Id = Guid.NewGuid() }
        };
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento { Id = Guid.NewGuid() }
        };

        // Act
        var contexto = new CalculoRiscoContexto(investimentos, produtos, DateTime.UtcNow);

        // Assert
        contexto.Investimentos.Should().BeSameAs(investimentos);
        contexto.Produtos.Should().BeSameAs(produtos);
    }
}