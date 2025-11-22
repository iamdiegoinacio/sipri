using FluentAssertions;
using Moq;
using SIPRI.Application.Interfaces;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Application.Handlers.Perfis;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Application.Tests.UseCases.Perfis;

public class GetPerfilRiscoHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCalculateAndReturnProfile()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };
        var perfilEsperado = PerfilRisco.Create(50); // Moderado

        var mockInvestRepo = new Mock<IInvestimentoRepository>();
        mockInvestRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

        var mockMotor = new Mock<IMotorPerfilRiscoServico>();
        mockMotor.Setup(m => m.CalcularPerfil(
            It.IsAny<IReadOnlyCollection<Investimento>>(),
            It.IsAny<IReadOnlyCollection<ProdutoInvestimento>>(),
            It.IsAny<DateTime>()))
            .Returns(perfilEsperado);

        var mockDate = new Mock<IDateTimeProvider>();
        mockDate.Setup(d => d.Today).Returns(DateTime.UtcNow);

        var handler = new GetPerfilRiscoHandler(
            mockInvestRepo.Object,
            mockProdRepo.Object,
            mockMotor.Object,
            mockDate.Object);

        var query = new GetPerfilRiscoQuery(clienteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ClienteId.Should().Be(clienteId);
        result.Perfil.Should().Be(perfilEsperado.Perfil);
        result.Pontuacao.Should().Be(perfilEsperado.Pontuacao);
        result.Descricao.Should().Be(perfilEsperado.Descricao);
    }
}

