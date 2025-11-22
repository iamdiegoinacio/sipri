using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Commands.Simulacoes; using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Presentation.Controllers;

namespace SIPRI.Presentation.Tests.Controllers;

public class SimulacaoControllerTests
{
    [Fact]
    public async Task SimularInvestimento_ShouldReturnOk_WhenCommandIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var request = new SimulacaoRequestDto { ClienteId = Guid.NewGuid(), Valor = 1000, PrazoMeses = 12, TipoProduto = "CDB" };
        var expectedResult = new SimulacaoResponseDto { DataSimulacao = DateTime.UtcNow };

        mediatorMock.Setup(m => m.Send(It.IsAny<SimularInvestimentoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new SimulacaoController(mediatorMock.Object);

        // Act
        var result = await controller.SimularInvestimento(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetSimulacoes_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var clienteId = Guid.NewGuid();
        var expectedResult = new List<HistoricoSimulacaoDto>
        {
            new HistoricoSimulacaoDto { Id = Guid.NewGuid(), ValorInvestido = 1000 }
        };

        mediatorMock.Setup(m => m.Send(It.IsAny<GetSimulacoesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new SimulacaoController(mediatorMock.Object);

        // Act
        var result = await controller.GetSimulacoes(clienteId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetSimulacoesAgregadas_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expectedResult = new List<SimulacaoAgregadaDto>
        {
            new SimulacaoAgregadaDto { Produto = "CDB", QuantidadeSimulacoes = 10 }
        };

        mediatorMock.Setup(m => m.Send(It.IsAny<GetSimulacoesAgregadasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new SimulacaoController(mediatorMock.Object);

        // Act
        var result = await controller.GetSimulacoesAgregadas(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}
