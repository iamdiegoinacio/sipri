using FluentAssertions;
using Moq;
using SIPRI.Application.Handlers.Simulacoes;
using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Simulacoes;

public class GetSimulacoesHandlerTests_Extended
{
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSimulationsFound()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(new List<Simulacao>());

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapAllProperties_Correctly()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var simulacaoId = Guid.NewGuid();
        var dataSimulacao = DateTime.UtcNow.AddDays(-5);

        var simulacoes = new List<Simulacao>
        {
            new Simulacao
            {
                Id = simulacaoId,
                ClienteId = clienteId,
                ProdutoNome = "CDB Premium",
                ValorInvestido = 5000m,
                ValorFinal = 5500m,
                PrazoMeses = 24,
                DataSimulacao = dataSimulacao
            }
        };

        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(simulacoes);

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Id.Should().Be(simulacaoId);
        dto.ClienteId.Should().Be(clienteId);
        dto.Produto.Should().Be("CDB Premium");
        dto.ValorInvestido.Should().Be(5000m);
        dto.ValorFinal.Should().Be(5500m);
        dto.PrazoMeses.Should().Be(24);
        dto.DataSimulacao.Should().Be(dataSimulacao);
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleSimulations_WhenAvailable()
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
                DataSimulacao = DateTime.UtcNow.AddDays(-10)
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                ProdutoNome = "Fundo",
                ValorInvestido = 2000,
                ValorFinal = 2300,
                PrazoMeses = 18,
                DataSimulacao = DateTime.UtcNow.AddDays(-5)
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                ProdutoNome = "Ações",
                ValorInvestido = 3000,
                ValorFinal = 3900,
                PrazoMeses = 24,
                DataSimulacao = DateTime.UtcNow.AddDays(-1)
            }
        };

        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(simulacoes);

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Produto == "CDB");
        result.Should().Contain(s => s.Produto == "Fundo");
        result.Should().Contain(s => s.Produto == "Ações");
    }

    [Fact]
    public async Task Handle_ShouldPreserveOrder_FromRepository()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var simulacoes = new List<Simulacao>
        {
            new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId, ProdutoNome = "Primeiro", ValorInvestido = 1000, ValorFinal = 1100, PrazoMeses = 12, DataSimulacao = DateTime.UtcNow.AddDays(-3) },
            new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId, ProdutoNome = "Segundo", ValorInvestido = 2000, ValorFinal = 2200, PrazoMeses = 12, DataSimulacao = DateTime.UtcNow.AddDays(-2) },
            new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId, ProdutoNome = "Terceiro", ValorInvestido = 3000, ValorFinal = 3300, PrazoMeses = 12, DataSimulacao = DateTime.UtcNow.AddDays(-1) }
        };

        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(simulacoes);

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.ElementAt(0).Produto.Should().Be("Primeiro");
        result.ElementAt(1).Produto.Should().Be("Segundo");
        result.ElementAt(2).Produto.Should().Be("Terceiro");
    }
}