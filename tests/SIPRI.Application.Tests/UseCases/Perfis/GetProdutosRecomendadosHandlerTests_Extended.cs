using FluentAssertions;
using Moq;
using SIPRI.Application.Handlers.Perfis;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Tests.UseCases.Perfis;

public class GetProdutosRecomendadosHandlerTests_Extended
{
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange
        var perfil = "Conservador";
        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(perfil))
            .ReturnsAsync(new List<ProdutoInvestimento>());

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery(perfil);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapAllProductProperties_Correctly()
    {
        // Arrange
        var perfil = "Moderado";
        var produtoId = Guid.NewGuid();
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento 
            { 
                Id = produtoId, 
                Nome = "Fundo Moderado", 
                Tipo = "Fundo", 
                RentabilidadeBase = 0.15m,
                Risco = "Medio"
            }
        };

        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(perfil)).ReturnsAsync(produtos);

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery(perfil);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Id.Should().Be(produtoId);
        dto.Nome.Should().Be("Fundo Moderado");
        dto.Tipo.Should().Be("Fundo");
        dto.Rentabilidade.Should().Be(0.15m);
        dto.Risco.Should().Be("Medio");
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleProducts_WhenAvailable()
    {
        // Arrange
        var perfil = "Agressivo";
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Ações", Tipo = "Acao", RentabilidadeBase = 0.25m, Risco = "Alto" },
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Fundo Multimercado", Tipo = "Fundo", RentabilidadeBase = 0.20m, Risco = "Alto" },
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Derivativos", Tipo = "Derivativo", RentabilidadeBase = 0.30m, Risco = "MuitoAlto" }
        };

        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(perfil)).ReturnsAsync(produtos);

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery(perfil);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Nome == "Ações");
        result.Should().Contain(p => p.Nome == "Fundo Multimercado");
        result.Should().Contain(p => p.Nome == "Derivativos");
    }

    [Theory]
    [InlineData("Conservador")]
    [InlineData("Moderado")]
    [InlineData("Agressivo")]
    public async Task Handle_ShouldCallRepository_WithCorrectPerfil(string perfil)
    {
        // Arrange
        var mockRepo = new Mock<IProdutoInvestimentoRepository>();
        mockRepo.Setup(r => r.GetByPerfilRiscoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ProdutoInvestimento>());

        var handler = new GetProdutosRecomendadosHandler(mockRepo.Object);
        var query = new GetProdutosRecomendadosQuery(perfil);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        mockRepo.Verify(r => r.GetByPerfilRiscoAsync(perfil), Times.Once);
    }
}