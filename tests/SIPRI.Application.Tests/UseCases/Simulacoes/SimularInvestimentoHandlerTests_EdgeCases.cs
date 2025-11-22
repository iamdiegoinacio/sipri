using FluentAssertions;
using Moq;
using SIPRI.Application.Commands.Simulacoes;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Exceptions;
using SIPRI.Application.Handlers.Simulacoes;
using SIPRI.Application.Interfaces;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Application.Tests.UseCases.Simulacoes;

public class SimularInvestimentoHandlerTests_EdgeCases
{
    

    [Fact]
    public async Task Handle_ShouldUseCorrectDateTime_FromProvider()
    {
        // Arrange
        var dataEsperada = new DateTime(2024, 11, 21, 10, 30, 0, DateTimeKind.Utc);
        var requestData = new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 1000,
            PrazoMeses = 12
        };

        var produto = new ProdutoInvestimento 
        { 
            Id = Guid.NewGuid(), 
            Nome = "CDB", 
            Tipo = "CDB", 
            RentabilidadeBase = 0.1m 
        };

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetByTipoAsync(requestData.TipoProduto)).ReturnsAsync(produto);

        var mockSimRepo = new Mock<ISimulacaoRepository>();
        var mockCalcService = new Mock<ICalculadoraInvestimentoService>();
        mockCalcService.Setup(s => s.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(1100m);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        
        var mockDate = new Mock<IDateTimeProvider>();
        mockDate.Setup(d => d.UtcNow).Returns(dataEsperada);

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
        result.DataSimulacao.Should().Be(dataEsperada);
    }


    [Fact]
    public async Task Handle_ShouldPersistSimulationWithCorrectData()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var requestData = new SimulacaoRequestDto
        {
            ClienteId = clienteId,
            TipoProduto = "CDB",
            Valor = 1000m,
            PrazoMeses = 12
        };

        var produto = new ProdutoInvestimento 
        { 
            Id = Guid.NewGuid(), 
            Nome = "CDB Teste", 
            Tipo = "CDB", 
            RentabilidadeBase = 0.1m 
        };

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetByTipoAsync(requestData.TipoProduto)).ReturnsAsync(produto);

        Simulacao? capturedSimulacao = null;
        var mockSimRepo = new Mock<ISimulacaoRepository>();
        mockSimRepo.Setup(r => r.AddAsync(It.IsAny<Simulacao>()))
            .Callback<Simulacao>(s => capturedSimulacao = s)
            .Returns(Task.CompletedTask);

        var mockCalcService = new Mock<ICalculadoraInvestimentoService>();
        mockCalcService.Setup(s => s.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(1100m);

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
        await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedSimulacao.Should().NotBeNull();
        capturedSimulacao!.ClienteId.Should().Be(clienteId);
        capturedSimulacao.ProdutoNome.Should().Be("CDB Teste");
        capturedSimulacao.ValorInvestido.Should().Be(1000m);
        capturedSimulacao.ValorFinal.Should().Be(1100m);
        capturedSimulacao.PrazoMeses.Should().Be(12);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChanges_AfterAddingSimulation()
    {
        // Arrange
        var requestData = new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 1000m,
            PrazoMeses = 12
        };

        var produto = new ProdutoInvestimento 
        { 
            Id = Guid.NewGuid(), 
            Nome = "CDB", 
            Tipo = "CDB", 
            RentabilidadeBase = 0.1m 
        };

        var mockProdRepo = new Mock<IProdutoInvestimentoRepository>();
        mockProdRepo.Setup(r => r.GetByTipoAsync(requestData.TipoProduto)).ReturnsAsync(produto);

        var mockSimRepo = new Mock<ISimulacaoRepository>();
        var mockCalcService = new Mock<ICalculadoraInvestimentoService>();
        mockCalcService.Setup(s => s.Calcular(It.IsAny<CalculoInvestimentoContexto>())).Returns(1100m);

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
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockSimRepo.Verify(r => r.AddAsync(It.IsAny<Simulacao>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}