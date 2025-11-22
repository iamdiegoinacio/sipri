using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.Queries.Perfis;
using SIPRI.Presentation.Controllers;

namespace SIPRI.Presentation.Tests.Controllers;

public class PerfilControllerTests
{
    [Fact]
    public async Task GetPerfilRisco_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var clienteId = Guid.NewGuid();
        var expectedResult = new PerfilRiscoDto { Perfil = "Moderado", Pontuacao = 50 };

        mediatorMock.Setup(m => m.Send(It.IsAny<GetPerfilRiscoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new PerfilController(mediatorMock.Object);

        // Act
        var result = await controller.GetPerfilRisco(clienteId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetProdutosRecomendados_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var perfil = "Moderado";
        var expectedResult = new List<ProdutoRecomendadoDto>
        {
            new ProdutoRecomendadoDto { Nome = "Fundo X", Tipo = "Fundo" }
        };

        mediatorMock.Setup(m => m.Send(It.IsAny<GetProdutosRecomendadosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new PerfilController(mediatorMock.Object);

        // Act
        var result = await controller.GetProdutosRecomendados(perfil, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}
