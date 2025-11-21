using FluentAssertions;
using Moq;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Exceptions;
using SIPRI.Application.Interfaces;
using SIPRI.Application.UseCases.Simulacoes;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Application.Tests.UseCases.Simulacoes;

public class SimularInvestimentoHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCalculateAndPersistSimulation_WhenProductExists()
    {
        // Arrange
        var requestData = new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 1000,
            PrazoMeses = 12
        };

        var produto = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "CDB Caixa", Tipo = "CDB", RentabilidadeBase = 0.1m };
        var valorFinalEsperado = 1100m;

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetByTipoAsync(requestData.TipoProduto)).ReturnsAsync(produto);

        var mockSimRepo = new Mock<ISimulacaoRepository>();
        
        var mockCalcService = new Mock<ICalculadoraInvestimentoService>();
        mockCalcService.Setup(s => s.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(valorFinalEsperado);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        
        var mockDate = new Mock<IDateTimeProvider>();
        mockDate.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

        var handler = new SimularInvestimentoHandler(
            mockProdRepo.Object,
            mockSimRepo.Object,
            mockCalcService.Object,
            mockUnitOfWork.Object,
            mockDate.Object);

        var command = new SimularInvestimentoCommand(requestData);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ResultadoSimulacao.ValorFinal.Should().Be(valorFinalEsperado);
        result.ProdutoValidado.Nome.Should().Be(produto.Nome);

        mockSimRepo.Verify(r => r.AddAsync(It.IsAny<Simulacao>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var requestData = new SimulacaoRequestDto { TipoProduto = "Inexistente" };

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetByTipoAsync(requestData.TipoProduto)).ReturnsAsync((ProdutoInvestimento?)null);

        var handler = new SimularInvestimentoHandler(
            mockProdRepo.Object,
            new Mock<ISimulacaoRepository>().Object,
            new Mock<ICalculadoraInvestimentoService>().Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IDateTimeProvider>().Object);

        var command = new SimularInvestimentoCommand(requestData);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
