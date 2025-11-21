using FluentAssertions;
using SIPRI.Infrastructure.Services;

namespace SIPRI.Infrastructure.Tests.Services;

public class TelemetryServiceTests
{
    [Fact]
    public async Task GetMetricsAsync_ShouldReturnEmpty_WhenNoRequestsRecorded()
    {
        // Arrange
        var service = new TelemetryService();

        // Act
        var result = await service.GetMetricsAsync();

        // Assert
        result.Servicos.Should().BeEmpty();
        result.Periodo.Inicio.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task RecordRequest_ShouldUpdateStats()
    {
        // Arrange
        var service = new TelemetryService();
        var endpoint = "GET /api/test";

        // Act
        service.RecordRequest(endpoint, 100); // 1st call, 100ms
        service.RecordRequest(endpoint, 200); // 2nd call, 200ms

        var metrics = await service.GetMetricsAsync();

        // Assert
        metrics.Servicos.Should().HaveCount(1);
        var stats = metrics.Servicos.First();
        
        stats.Nome.Should().Be(endpoint);
        stats.QuantidadeChamadas.Should().Be(2);
        stats.MediaTempoRespostaMs.Should().Be(150); // (100+200)/2
    }

    [Fact]
    public async Task RecordRequest_ShouldTrackMultipleEndpointsSeparately()
    {
        // Arrange
        var service = new TelemetryService();

        // Act
        service.RecordRequest("A", 10);
        service.RecordRequest("B", 20);

        var metrics = await service.GetMetricsAsync();

        // Assert
        metrics.Servicos.Should().HaveCount(2);
        metrics.Servicos.Should().Contain(s => s.Nome == "A" && s.QuantidadeChamadas == 1);
        metrics.Servicos.Should().Contain(s => s.Nome == "B" && s.QuantidadeChamadas == 1);
    }
}
