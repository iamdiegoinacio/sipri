using FluentAssertions;
using Moq;
using SIPRI.Application.Handlers.Perfis;
using SIPRI.Application.Interfaces;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Application.Tests.UseCases.Perfis;

public class GetPerfilRiscoHandlerTests_EdgeCases
{
    [Fact]
    public async Task Handle_ShouldReturnConservadorPadrao_WhenNoInvestments()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };
        var perfilConservador = PerfilRisco.ConservadorPadrao;

        var mockInvestRepo = new Mock<IInvestimentoRepository>();
        mockInvestRepo.Setup(r => r.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(new List<Investimento>());

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

        var mockMotor = new Mock<IMotorPerfilRiscoServico>();
        mockMotor.Setup(m => m.CalcularPerfil(
            It.IsAny<IReadOnlyCollection<Investimento>>(),
            It.IsAny<IReadOnlyCollection<ProdutoInvestimento>>(),
            It.IsAny<DateTime>()))
            .Returns(perfilConservador);

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
        result.Perfil.Should().Be("Conservador");
        result.Pontuacao.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldUseCorrectDate_FromProvider()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var dataEsperada = new DateTime(2024, 11, 21);
        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };

        var mockInvestRepo = new Mock<IInvestimentoRepository>();
        mockInvestRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

        DateTime capturedDate = DateTime.MinValue;
        var mockMotor = new Mock<IMotorPerfilRiscoServico>();
        mockMotor.Setup(m => m.CalcularPerfil(
            It.IsAny<IReadOnlyCollection<Investimento>>(),
            It.IsAny<IReadOnlyCollection<ProdutoInvestimento>>(),
            It.IsAny<DateTime>()))
            .Callback<IReadOnlyCollection<Investimento>, IReadOnlyCollection<ProdutoInvestimento>, DateTime>(
                (inv, prod, date) => capturedDate = date)
            .Returns(PerfilRisco.Create(50));

        var mockDate = new Mock<IDateTimeProvider>();
        mockDate.Setup(d => d.Today).Returns(dataEsperada);

        var handler = new GetPerfilRiscoHandler(
            mockInvestRepo.Object,
            mockProdRepo.Object,
            mockMotor.Object,
            mockDate.Object);

        var query = new GetPerfilRiscoQuery(clienteId);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        capturedDate.Should().Be(dataEsperada);
    }

    [Theory]
    [InlineData(30, "Conservador")]
    [InlineData(50, "Moderado")]
    [InlineData(80, "Agressivo")]
    public async Task Handle_ShouldReturnCorrectPerfilType(int pontuacao, string perfilEsperado)
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var investimentos = new List<Investimento> { new Investimento() };
        var produtos = new List<ProdutoInvestimento> { new ProdutoInvestimento() };
        var perfil = PerfilRisco.Create(pontuacao);

        var mockInvestRepo = new Mock<IInvestimentoRepository>();
        mockInvestRepo.Setup(r => r.GetByClienteIdAsync(clienteId)).ReturnsAsync(investimentos);

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

        var mockMotor = new Mock<IMotorPerfilRiscoServico>();
        mockMotor.Setup(m => m.CalcularPerfil(
            It.IsAny<IReadOnlyCollection<Investimento>>(),
            It.IsAny<IReadOnlyCollection<ProdutoInvestimento>>(),
            It.IsAny<DateTime>()))
            .Returns(perfil);

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
        result.Perfil.Should().Be(perfilEsperado);
        result.Pontuacao.Should().Be(pontuacao);
    }
}