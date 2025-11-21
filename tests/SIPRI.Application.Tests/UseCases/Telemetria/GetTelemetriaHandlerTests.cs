using FluentAssertions;
using Moq;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.Interfaces;
using SIPRI.Application.UseCases.Telemetria;

namespace SIPRI.Application.Tests.UseCases.Telemetria;

public class GetTelemetriaHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMetrics_FromService()
    {
        // Arrange
        var expectedDto = new TelemetriaDto
        {
            Servicos = new List<ServicoTelemetriaDto> 
            { 
                new ServicoTelemetriaDto { Nome = "API", QuantidadeChamadas = 10, MediaTempoRespostaMs = 50 } 
            },
            Periodo = new PeriodoTelemetriaDto 
            { 
                Inicio = DateOnly.FromDateTime(DateTime.UtcNow), 
                Fim = DateOnly.FromDateTime(DateTime.UtcNow) 
            }
        };

        var mockService = new Mock<ITelemetryService>();
        mockService.Setup(s => s.GetMetricsAsync()).ReturnsAsync(expectedDto);

        var handler = new GetTelemetriaHandler(mockService.Object);
        var query = new GetTelemetriaQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(expectedDto);
        result.Servicos.Should().HaveCount(1);
        result.Servicos.First().Nome.Should().Be("API");
    }
}
