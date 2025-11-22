using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class InvestimentoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var investimento = new Investimento();

        // Assert
        investimento.Id.Should().Be(Guid.Empty);
        investimento.ClienteId.Should().Be(Guid.Empty);
        investimento.ProdutoId.Should().Be(Guid.Empty);
        investimento.Tipo.Should().BeEmpty();
        investimento.Valor.Should().Be(0);
        investimento.Rentabilidade.Should().Be(0);
        investimento.Data.Should().Be(default(DateTime));
    }

    [Fact]
    public void Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var tipo = "CDB";
        var valor = 1000m;
        var rentabilidade = 0.12m;
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

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(1000.50)]
    [InlineData(999999.99)]
    public void Valor_ShouldAcceptValidDecimalValues(decimal valor)
    {
        // Act
        var investimento = new Investimento { Valor = valor };

        // Assert
        investimento.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.05)]
    [InlineData(0.12)]
    [InlineData(1.0)]
    public void Rentabilidade_ShouldAcceptValidPercentageValues(decimal rentabilidade)
    {
        // Act
        var investimento = new Investimento { Rentabilidade = rentabilidade };

        // Assert
        investimento.Rentabilidade.Should().Be(rentabilidade);
    }

    [Fact]
    public void Tipo_ShouldAcceptDifferentProductTypes()
    {
        // Arrange
        var tipos = new[] { "CDB", "Fundo", "LCI", "LCA", "Tesouro Direto" };

        foreach (var tipo in tipos)
        {
            // Act
            var investimento = new Investimento { Tipo = tipo };

            // Assert
            investimento.Tipo.Should().Be(tipo);
        }
    }
}
