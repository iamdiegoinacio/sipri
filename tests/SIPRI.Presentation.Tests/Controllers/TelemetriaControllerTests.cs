using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.UseCases.Telemetria;
using SIPRI.Presentation.Controllers;

namespace SIPRI.Presentation.Tests.Controllers;

public class TelemetriaControllerTests
{
    [Fact]
    public async Task GetTelemetria_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expectedResult = new TelemetriaDto();

        mediatorMock.Setup(m => m.Send(It.IsAny<GetTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new TelemetriaController(mediatorMock.Object);

        // Act
        var result = await controller.GetTelemetria(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}
