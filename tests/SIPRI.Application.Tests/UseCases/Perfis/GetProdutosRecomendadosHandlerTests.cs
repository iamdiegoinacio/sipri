using FluentAssertions;
using Moq;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Application.Handlers.Perfis;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Perfis;

public class GetProdutosRecomendadosHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnRecommendedProducts()
    {
        // Arrange
        var perfil = "Conservador";
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "CDB", Tipo = "CDB", RentabilidadeBase = 0.1m, Risco = "Baixo" }
        };

        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(perfil)).ReturnsAsync(produtos);

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery(perfil);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Nome.Should().Be("CDB");
    }
}

