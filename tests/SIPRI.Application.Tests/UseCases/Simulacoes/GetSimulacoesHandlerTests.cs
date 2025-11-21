using FluentAssertions;
using Moq;
using SIPRI.Application.Commands.Simulacoes;
using SIPRI.Application.Handlers.Simulacoes; using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Application.Handlers.Simulacoes;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Simulacoes;

public class GetSimulacoesHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMappedSimulations()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var simulacoes = new List<Simulacao>
        {
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                ProdutoNome = "CDB",
                ValorInvestido = 1000,
                ValorFinal = 1100,
                PrazoMeses = 12,
                DataSimulacao = DateTime.UtcNow
            }
        };

        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(simulacoes);

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Produto.Should().Be("CDB");
        result.First().ValorFinal.Should().Be(1100);
    }
}

