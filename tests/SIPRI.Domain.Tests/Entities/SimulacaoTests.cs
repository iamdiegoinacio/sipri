using FluentAssertions;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Entities;

public class SimulacaoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var simulacao = new Simulacao();

        // Assert
        simulacao.Id.Should().Be(Guid.Empty);
        simulacao.ClienteId.Should().Be(Guid.Empty);
        simulacao.ProdutoId.Should().Be(Guid.Empty);
        simulacao.ProdutoNome.Should().BeEmpty();
        simulacao.ValorInvestido.Should().Be(0);
        simulacao.PrazoMeses.Should().Be(0);
        simulacao.ValorFinal.Should().Be(0);
        simulacao.DataSimulacao.Should().Be(default(DateTime));
    }

    [Fact]
    public void Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var produtoNome = "CDB Caixa 2026";
        var valorInvestido = 1000m;
        var prazoMeses = 12;
        var valorFinal = 1120m;
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

    [Theory]
    [InlineData(100, 12, 110)]
    [InlineData(1000, 6, 1060)]
    [InlineData(5000, 24, 6200)]
    public void ValorFinal_ShouldBeGreaterThanValorInvestido(decimal valorInvestido, int prazo, decimal valorFinal)
    {
        // Act
        var simulacao = new Simulacao
        {
            ValorInvestido = valorInvestido,
            PrazoMeses = prazo,
            ValorFinal = valorFinal
        };

        // Assert
        simulacao.ValorFinal.Should().BeGreaterThan(simulacao.ValorInvestido);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public void PrazoMeses_ShouldAcceptValidDurations(int prazoMeses)
    {
        // Act
        var simulacao = new Simulacao { PrazoMeses = prazoMeses };

        // Assert
        simulacao.PrazoMeses.Should().Be(prazoMeses);
    }

    [Fact]
    public void ProdutoNome_ShouldStoreDenormalizedData()
    {
        // Arrange
        var produtoNome = "CDB Prefixado 2025";

        // Act
        var simulacao = new Simulacao
        {
            ProdutoId = Guid.NewGuid(),
            ProdutoNome = produtoNome
        };

        // Assert
        simulacao.ProdutoNome.Should().Be(produtoNome);
        simulacao.ProdutoId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DataSimulacao_ShouldRecordWhenSimulationWasPerformed()
    {
        // Arrange
        var dataSimulacao = new DateTime(2025, 11, 21, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var simulacao = new Simulacao { DataSimulacao = dataSimulacao };

        // Assert
        simulacao.DataSimulacao.Should().Be(dataSimulacao);
        simulacao.DataSimulacao.Kind.Should().Be(DateTimeKind.Utc);
    }
}
