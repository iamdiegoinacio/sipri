using FluentAssertions;
using Moq;
using SIPRI.Application.Handlers.Investimentos;
using SIPRI.Application.Handlers.Perfis;
using SIPRI.Application.Handlers.Simulacoes;
using SIPRI.Application.Queries.Investimentos;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Common;

public class CancellationTokenTests
{
    [Fact]
    public async Task GetInvestimentos_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var mockRepo = new Mock<IInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(It.IsAny<Guid>()))
            .Returns(async () =>
            {
                await Task.Delay(100);
                cts.Token.ThrowIfCancellationRequested();
                return new List<Investimento>();
            });

        var handler = new GetInvestimentosHandler(mockRepo.Object);
        var query = new GetInvestimentosQuery(Guid.NewGuid());

        cts.Cancel();

        // Act
        var act = async () => await handler.Handle(query, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetSimulacoes_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var mockRepo = new Mock<ISimulacaoRepository>();
        mockRepo.Setup(r => r.GetByClienteIdAsync(It.IsAny<Guid>()))
            .Returns(async () =>
            {
                await Task.Delay(100);
                cts.Token.ThrowIfCancellationRequested();
                return new List<Simulacao>();
            });

        var handler = new GetSimulacoesHandler(mockRepo.Object);
        var query = new GetSimulacoesQuery(Guid.NewGuid());

        cts.Cancel();

        // Act
        var act = async () => await handler.Handle(query, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetProdutosRecomendados_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(It.IsAny<string>()))
            .Returns(async () =>
            {
                await Task.Delay(100);
                cts.Token.ThrowIfCancellationRequested();
                return new List<ProdutoInvestimento>();
            });

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery("Conservador");

        cts.Cancel();

        // Act
        var act = async () => await handler.Handle(query, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}