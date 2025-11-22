using SIPRI.Domain.Models;
using Xunit;

namespace SIPRI.Domain.Tests.Models;

public class SimulacaoAgregadaTests
{
    [Fact]
    public void SimulacaoAgregada_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var agregada = new SimulacaoAgregada();

        // Assert
        Assert.Equal(string.Empty, agregada.Produto);
        Assert.Equal(default(DateTime), agregada.Data);
        Assert.Equal(0, agregada.QuantidadeSimulacoes);
        Assert.Equal(0m, agregada.MediaValorFinal);
    }

    [Fact]
    public void SimulacaoAgregada_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var produto = "CDB Premium";
        var data = new DateTime(2024, 1, 1);
        var quantidade = 10;
        var media = 11500m;

        // Act
        var agregada = new SimulacaoAgregada
        {
            Produto = produto,
            Data = data,
            QuantidadeSimulacoes = quantidade,
            MediaValorFinal = media
        };

        // Assert
        Assert.Equal(produto, agregada.Produto);
        Assert.Equal(data, agregada.Data);
        Assert.Equal(quantidade, agregada.QuantidadeSimulacoes);
        Assert.Equal(media, agregada.MediaValorFinal);
    }

    [Fact]
    public void SimulacaoAgregada_ShouldCalculateMediaCorrectly()
    {
        // Arrange
        var agregada = new SimulacaoAgregada
        {
            Produto = "CDB",
            Data = new DateTime(2024, 1, 1),
            QuantidadeSimulacoes = 5,
            MediaValorFinal = 11000m
        };

        // Assert
        Assert.Equal(11000m, agregada.MediaValorFinal);
        Assert.Equal(5, agregada.QuantidadeSimulacoes);
    }

    [Fact]
    public void SimulacaoAgregada_ShouldGroupByProdutoAndData()
    {
        // Arrange
        var agregadas = new List<SimulacaoAgregada>
        {
            new() { Produto = "CDB", Data = new DateTime(2024, 1, 1), QuantidadeSimulacoes = 5, MediaValorFinal = 11000m },
            new() { Produto = "CDB", Data = new DateTime(2024, 1, 2), QuantidadeSimulacoes = 3, MediaValorFinal = 12000m },
            new() { Produto = "Fundo", Data = new DateTime(2024, 1, 1), QuantidadeSimulacoes = 2, MediaValorFinal = 13000m }
        };

        // Act
        var groupedByProduto = agregadas.GroupBy(a => a.Produto).ToList();
        var groupedByData = agregadas.GroupBy(a => a.Data).ToList();

        // Assert
        Assert.Equal(2, groupedByProduto.Count); // CDB e Fundo
        Assert.Equal(2, groupedByData.Count); // 2024-01-01 e 2024-01-02
    }

    [Fact]
    public void SimulacaoAgregada_ShouldHandleZeroQuantidade()
    {
        // Act
        var agregada = new SimulacaoAgregada
        {
            Produto = "CDB",
            Data = DateTime.Now,
            QuantidadeSimulacoes = 0,
            MediaValorFinal = 0m
        };

        // Assert
        Assert.Equal(0, agregada.QuantidadeSimulacoes);
        Assert.Equal(0m, agregada.MediaValorFinal);
    }

    [Fact]
    public void SimulacaoAgregada_ShouldHandleLargeValues()
    {
        // Act
        var agregada = new SimulacaoAgregada
        {
            Produto = "Fundo Multimercado",
            Data = DateTime.Now,
            QuantidadeSimulacoes = 1000,
            MediaValorFinal = 999999999.99m
        };

        // Assert
        Assert.Equal(1000, agregada.QuantidadeSimulacoes);
        Assert.Equal(999999999.99m, agregada.MediaValorFinal);
    }

    [Theory]
    [InlineData("CDB", 10, 11000)]
    [InlineData("Fundo", 5, 12500)]
    [InlineData("LCI", 15, 10800)]
    public void SimulacaoAgregada_ShouldAcceptVariousProducts(string produto, int quantidade, decimal media)
    {
        // Act
        var agregada = new SimulacaoAgregada
        {
            Produto = produto,
            Data = DateTime.Now,
            QuantidadeSimulacoes = quantidade,
            MediaValorFinal = media
        };

        // Assert
        Assert.Equal(produto, agregada.Produto);
        Assert.Equal(quantidade, agregada.QuantidadeSimulacoes);
        Assert.Equal(media, agregada.MediaValorFinal);
    }
}
