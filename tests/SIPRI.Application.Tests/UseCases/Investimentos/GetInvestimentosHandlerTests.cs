using FluentAssertions;
using Moq;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Application.Queries.Investimentos;
using SIPRI.Application.Handlers.Investimentos;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Investimentos;

public class GetInvestimentosHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMappedInvestments_WhenInvestmentsExist()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentos = new List<Investimento>
        {
            new Investimento
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                Tipo = "CDB",
                Valor = 1000,
                Rentabilidade = 0.1m,
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
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(investimentos[0].Id);
        result.First().Tipo.Should().Be("CDB");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoInvestmentsFound()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(new List<Investimento>());

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}

