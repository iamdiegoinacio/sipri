using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class ProdutoInvestimentoTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var produto = new ProdutoInvestimento();

        // Assert
        produto.Id.Should().Be(Guid.Empty);
        produto.Nome.Should().BeEmpty();
        produto.Tipo.Should().BeEmpty();
        produto.RentabilidadeBase.Should().Be(0);
        produto.Risco.Should().BeEmpty();
        produto.NivelRisco.Should().Be(0);
    }

    [Fact]
    public void ParameterizedConstructor_ShouldInitializeAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nome = "CDB Caixa 2026";
        var tipo = "CDB";
        var rentabilidade = 0.12m;
        var risco = "Baixo";
        var nivelRisco = 1;

        // Act
        var produto = new ProdutoInvestimento(id, nome, tipo, rentabilidade, risco, nivelRisco);

        // Assert
        produto.Id.Should().Be(id);
        produto.Nome.Should().Be(nome);
        produto.Tipo.Should().Be(tipo);
        produto.RentabilidadeBase.Should().Be(rentabilidade);
        produto.Risco.Should().Be(risco);
        produto.NivelRisco.Should().Be(nivelRisco);
    }

    [Theory]
    [InlineData("Baixo", 1)]
    [InlineData("Moderado", 2)]
    [InlineData("Alto", 3)]
    public void NivelRisco_ShouldMatchRiscoDescription(string risco, int nivelRisco)
    {
        // Act
        var produto = new ProdutoInvestimento
        {
            Risco = risco,
            NivelRisco = nivelRisco
        };

        // Assert
        produto.Risco.Should().Be(risco);
        produto.NivelRisco.Should().Be(nivelRisco);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.05)]
    [InlineData(0.12)]
    [InlineData(0.25)]
    [InlineData(1.0)]
    public void RentabilidadeBase_ShouldAcceptValidValues(decimal rentabilidade)
    {
        // Act
        var produto = new ProdutoInvestimento { RentabilidadeBase = rentabilidade };

        // Assert
        produto.RentabilidadeBase.Should().Be(rentabilidade);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("Fundo")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    public void Tipo_ShouldAcceptDifferentProductTypes(string tipo)
    {
        // Act
        var produto = new ProdutoInvestimento { Tipo = tipo };

        // Assert
        produto.Tipo.Should().Be(tipo);
    }

    [Fact]
    public void Properties_ShouldBeIndependentlyModifiable()
    {
        // Arrange
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Teste"
        };

        // Act
        produto.Tipo = "CDB";
        produto.RentabilidadeBase = 0.10m;
        produto.Risco = "Moderado";
        produto.NivelRisco = 2;

        // Assert
        produto.Tipo.Should().Be("CDB");
        produto.RentabilidadeBase.Should().Be(0.10m);
        produto.Risco.Should().Be("Moderado");
        produto.NivelRisco.Should().Be(2);
    }
}
