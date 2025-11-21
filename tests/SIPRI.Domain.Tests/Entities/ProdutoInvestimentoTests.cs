using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class ProdutoInvestimentoTests
{
    [Fact]
    public void Constructor_WithParameters_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nome = "CDB Caixa";
        var tipo = "CDB";
        var rentabilidadeBase = 0.12m;
        var risco = "Baixo";
        var nivelRisco = 1;

        // Act
        var produto = new ProdutoInvestimento(id, nome, tipo, rentabilidadeBase, risco, nivelRisco);

        // Assert
        produto.Id.Should().Be(id);
        produto.Nome.Should().Be(nome);
        produto.Tipo.Should().Be(tipo);
        produto.RentabilidadeBase.Should().Be(rentabilidadeBase);
        produto.Risco.Should().Be(risco);
        produto.NivelRisco.Should().Be(nivelRisco);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var produto = new ProdutoInvestimento();

        // Assert
        produto.Should().NotBeNull();
        produto.Nome.Should().BeEmpty();
        produto.Tipo.Should().BeEmpty();
        produto.Risco.Should().BeEmpty();
    }
}
