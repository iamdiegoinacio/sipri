using FluentAssertions;
using Moq;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Handlers.Simulacoes;
using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Models;

namespace SIPRI.Application.Tests.Handlers.Simulacoes;

public class GetSimulacoesAgregadasHandlerTests
{
    private readonly Mock<ISimulacaoRepository> _mockSimulacaoRepository;
    private readonly GetSimulacoesAgregadasHandler _handler;

    public GetSimulacoesAgregadasHandlerTests()
    {
        _mockSimulacaoRepository = new Mock<ISimulacaoRepository>();
        _handler = new GetSimulacoesAgregadasHandler(_mockSimulacaoRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsData_ShouldReturnMappedDtos()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "CDB",
                Data = new DateTime(2024, 1, 15),
                QuantidadeSimulacoes = 10,
                MediaValorFinal = 11500m
            },
            new SimulacaoAgregada
            {
                Produto = "LCI",
                Data = new DateTime(2024, 1, 16),
                QuantidadeSimulacoes = 5,
                MediaValorFinal = 15000m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        
        resultList[0].Produto.Should().Be("CDB");
        resultList[0].Data.Should().Be(new DateOnly(2024, 1, 15));
        resultList[0].QuantidadeSimulacoes.Should().Be(10);
        resultList[0].MediaValorFinal.Should().Be(11500m);

        resultList[1].Produto.Should().Be("LCI");
        resultList[1].Data.Should().Be(new DateOnly(2024, 1, 16));
        resultList[1].QuantidadeSimulacoes.Should().Be(5);
        resultList[1].MediaValorFinal.Should().Be(15000m);

        _mockSimulacaoRepository.Verify(x => x.GetAgregadoPorDiaAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(new List<SimulacaoAgregada>());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockSimulacaoRepository.Verify(x => x.GetAgregadoPorDiaAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapDateTimeToDateOnlyCorrectly()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "CDB Premium",
                Data = new DateTime(2024, 12, 31, 23, 59, 59),
                QuantidadeSimulacoes = 15,
                MediaValorFinal = 25000m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        var resultItem = result.First();
        resultItem.Data.Should().Be(new DateOnly(2024, 12, 31));
        resultItem.Data.Year.Should().Be(2024);
        resultItem.Data.Month.Should().Be(12);
        resultItem.Data.Day.Should().Be(31);
    }

    [Fact]
    public async Task Handle_WithMultipleProductsOnSameDay_ShouldReturnAllRecords()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "CDB",
                Data = new DateTime(2024, 1, 15),
                QuantidadeSimulacoes = 10,
                MediaValorFinal = 11500m
            },
            new SimulacaoAgregada
            {
                Produto = "LCI",
                Data = new DateTime(2024, 1, 15),
                QuantidadeSimulacoes = 8,
                MediaValorFinal = 13000m
            },
            new SimulacaoAgregada
            {
                Produto = "LCA",
                Data = new DateTime(2024, 1, 15),
                QuantidadeSimulacoes = 3,
                MediaValorFinal = 9500m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(x => x.Data.Should().Be(new DateOnly(2024, 1, 15)));
        result.Select(x => x.Produto).Should().Contain(new[] { "CDB", "LCI", "LCA" });
    }

    [Fact]
    public async Task Handle_ShouldPreserveDecimalPrecision()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "Tesouro Direto",
                Data = new DateTime(2024, 1, 15),
                QuantidadeSimulacoes = 7,
                MediaValorFinal = 10123.456789m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        var resultItem = result.First();
        resultItem.MediaValorFinal.Should().Be(10123.456789m);
    }

    [Fact]
    public async Task Handle_ShouldHandleLargeQuantityOfRecords()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = Enumerable.Range(1, 100)
            .Select(i => new SimulacaoAgregada
            {
                Produto = $"Produto{i}",
                Data = new DateTime(2024, 1, 1).AddDays(i),
                QuantidadeSimulacoes = i,
                MediaValorFinal = 10000m + (i * 100)
            })
            .ToList();

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(100);
        var resultList = result.ToList();
        resultList[0].Produto.Should().Be("Produto1");
        resultList[99].Produto.Should().Be("Produto100");
        resultList[99].MediaValorFinal.Should().Be(20000m);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ThrowsAsync(new InvalidOperationException("Erro de banco de dados"));

        // Act
        Func<Task> act = async () => await _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro de banco de dados");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnlyOnce()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(new List<SimulacaoAgregada>());

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _mockSimulacaoRepository.Verify(x => x.GetAgregadoPorDiaAsync(), Times.Once);
        _mockSimulacaoRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithZeroQuantidadeSimulacoes_ShouldMapCorrectly()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "Produto Teste",
                Data = new DateTime(2024, 1, 1),
                QuantidadeSimulacoes = 0,
                MediaValorFinal = 0m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        var resultItem = result.First();
        resultItem.QuantidadeSimulacoes.Should().Be(0);
        resultItem.MediaValorFinal.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_WithNegativeValues_ShouldMapCorrectly()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        var dadosAgregados = new List<SimulacaoAgregada>
        {
            new SimulacaoAgregada
            {
                Produto = "Produto com Perda",
                Data = new DateTime(2024, 1, 1),
                QuantidadeSimulacoes = 5,
                MediaValorFinal = -1000m
            }
        };

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        var resultItem = result.First();
        resultItem.MediaValorFinal.Should().Be(-1000m);
    }

    [Fact]
    public async Task Handle_ShouldReturnIEnumerable()
    {
        // Arrange
        var query = new GetSimulacoesAgregadasQuery();
        var cancellationToken = CancellationToken.None;

        _mockSimulacaoRepository
            .Setup(x => x.GetAgregadoPorDiaAsync())
            .ReturnsAsync(new List<SimulacaoAgregada>());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeAssignableTo<IEnumerable<SimulacaoAgregadaDto>>();
    }
}