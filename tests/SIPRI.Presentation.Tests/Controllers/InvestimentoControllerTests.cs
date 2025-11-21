using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Application.UseCases.Investimentos;
using SIPRI.Presentation.Controllers;

namespace SIPRI.Presentation.Tests.Controllers;

public class InvestimentoControllerTests
{
    [Fact]
    public async Task GetInvestimentos_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var clienteId = Guid.NewGuid();
        var expectedResult = new List<HistoricoInvestimentoDto>
        {
            new HistoricoInvestimentoDto { Id = Guid.NewGuid(), Tipo = "CDB", Valor = 1000 }
        };

        mediatorMock.Setup(m => m.Send(It.IsAny<GetInvestimentosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new InvestimentoController(mediatorMock.Object);

        // Act
        var result = await controller.GetInvestimentos(clienteId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}
