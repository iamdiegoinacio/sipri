using FluentAssertions;
using Moq;
using SIPRI.Application.Handlers.Investimentos;
using SIPRI.Application.Queries.Investimentos;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Investimentos;

public class GetInvestimentosHandlerTests_Extended
{
    [Fact]
    public async Task Handle_ShouldMapAllProperties_Correctly()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentoId = Guid.NewGuid();
        var dataInvestimento = DateTime.UtcNow.AddDays(-30);

        var investimentos = new List<Investimento>
        {
            new Investimento
            {
                Id = investimentoId,
                ClienteId = clienteId,
                Tipo = "CDB Premium",
                Valor = 15000m,
                Rentabilidade = 0.125m,
                Data = dataInvestimento
            }
        };

        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Id.Should().Be(investimentoId);
        dto.Tipo.Should().Be("CDB Premium");
        dto.Valor.Should().Be(15000m);
        dto.Rentabilidade.Should().Be(0.125m);
        dto.Data.Should().Be(dataInvestimento);
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleInvestments_WhenAvailable()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentos = new List<Investimento>
        {
            new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Tipo = "CDB", Valor = 1000, Rentabilidade = 0.1m, Data = DateTime.UtcNow.AddDays(-60) },
            new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Tipo = "Fundo", Valor = 2000, Rentabilidade = 0.15m, Data = DateTime.UtcNow.AddDays(-45) },
            new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Tipo = "Ações", Valor = 3000, Rentabilidade = 0.20m, Data = DateTime.UtcNow.AddDays(-30) },
            new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Tipo = "Tesouro", Valor = 5000, Rentabilidade = 0.08m, Data = DateTime.UtcNow.AddDays(-15) }
        };

        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(i => i.Tipo == "CDB");
        result.Should().Contain(i => i.Tipo == "Fundo");
        result.Should().Contain(i => i.Tipo == "Ações");
        result.Should().Contain(i => i.Tipo == "Tesouro");
    }

    [Fact]
    public async Task Handle_ShouldCallRepository_WithCorrectClienteId()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Investimento>());

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(clienteId);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        mockRepo.Verify(r => r.GetByClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldHandleZeroValues_Correctly()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentos = new List<Investimento>
        {
            new Investimento
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                Tipo = "TestZero",
                Valor = 0m,
                Rentabilidade = 0m,
                Data = DateTime.UtcNow
            }
        };

        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Valor.Should().Be(0m);
        dto.Rentabilidade.Should().Be(0m);
    }
}