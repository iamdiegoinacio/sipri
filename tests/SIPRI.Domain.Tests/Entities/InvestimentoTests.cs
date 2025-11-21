using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class InvestimentoTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var tipo = "CDB";
        var valor = 1000m;
        var rentabilidade = 0.1m;
        var data = DateTime.UtcNow;

        // Act
        var investimento = new Investimento
        {
            Id = id,
            ClienteId = clienteId,
            ProdutoId = produtoId,
            Tipo = tipo,
            Valor = valor,
            Rentabilidade = rentabilidade,
            Data = data
        };

        // Assert
        investimento.Id.Should().Be(id);
        investimento.ClienteId.Should().Be(clienteId);
        investimento.ProdutoId.Should().Be(produtoId);
        investimento.Tipo.Should().Be(tipo);
        investimento.Valor.Should().Be(valor);
        investimento.Rentabilidade.Should().Be(rentabilidade);
        investimento.Data.Should().Be(data);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var investimento = new Investimento();

        // Assert
        investimento.Should().NotBeNull();
        investimento.Tipo.Should().BeEmpty();
    }
}
