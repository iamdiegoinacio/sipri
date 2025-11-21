using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class SimulacaoTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var produtoNome = "CDB Caixa";
        var valorInvestido = 5000m;
        var prazoMeses = 12;
        var valorFinal = 5600m;
        var dataSimulacao = DateTime.UtcNow;

        // Act
        var simulacao = new Simulacao
        {
            Id = id,
            ClienteId = clienteId,
            ProdutoId = produtoId,
            ProdutoNome = produtoNome,
            ValorInvestido = valorInvestido,
            PrazoMeses = prazoMeses,
            ValorFinal = valorFinal,
            DataSimulacao = dataSimulacao
        };

        // Assert
        simulacao.Id.Should().Be(id);
        simulacao.ClienteId.Should().Be(clienteId);
        simulacao.ProdutoId.Should().Be(produtoId);
        simulacao.ProdutoNome.Should().Be(produtoNome);
        simulacao.ValorInvestido.Should().Be(valorInvestido);
        simulacao.PrazoMeses.Should().Be(prazoMeses);
        simulacao.ValorFinal.Should().Be(valorFinal);
        simulacao.DataSimulacao.Should().Be(dataSimulacao);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var simulacao = new Simulacao();

        // Assert
        simulacao.Should().NotBeNull();
        simulacao.ProdutoNome.Should().BeEmpty();
    }
}
