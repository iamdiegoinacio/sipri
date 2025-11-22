using FluentAssertions;
using SIPRI.Infrastructure.Services;

namespace SIPRI.Infrastructure.Tests.Services;

public class TelemetryServiceExtendedTests
{
    [Fact]
    public async Task RecordRequest_ShouldHandleMultipleCallsToSameEndpoint()
    {
        // Arrange
        var service = new TelemetryService();
        var endpoint = "GET /api/test";

        // Act
        for (int i = 1; i <= 5; i++)
        {
            service.RecordRequest(endpoint, i * 100);
        }

        var metrics = await service.GetMetricsAsync();

        // Assert
        var stats = metrics.Servicos.First();
        stats.QuantidadeChamadas.Should().Be(5);
        stats.MediaTempoRespostaMs.Should().Be(300); // (100+200+300+400+500)/5
    }

    [Fact]
    public async Task RecordRequest_ShouldHandleZeroResponseTime()
    {
        // Arrange
        var service = new TelemetryService();

        // Act
        service.RecordRequest("endpoint", 0);
        var metrics = await service.GetMetricsAsync();

        // Assert
        var stats = metrics.Servicos.First();
        stats.MediaTempoRespostaMs.Should().Be(0);
    }

    [Fact]
    public async Task GetMetricsAsync_ShouldReturnCorrectPeriodoInicio()
    {
        // Arrange
        var service = new TelemetryService();
        var expectedDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var metrics = await service.GetMetricsAsync();

        // Assert
        metrics.Periodo.Inicio.Should().Be(expectedDate);
    }

    [Fact]
    public async Task RecordRequest_ShouldHandleLargeResponseTimes()
    {
        // Arrange
        var service = new TelemetryService();

        // Act
        service.RecordRequest("slow-endpoint", 10000); // 10 segundos
        var metrics = await service.GetMetricsAsync();

        // Assert
        var stats = metrics.Servicos.First();
        stats.MediaTempoRespostaMs.Should().Be(10000);
    }

    [Fact]
    public async Task RecordRequest_ShouldMaintainSeparateStatsPerEndpoint()
    {
        // Arrange
        var service = new TelemetryService();

        // Act
        service.RecordRequest("A", 100);
        service.RecordRequest("A", 200);
        service.RecordRequest("B", 300);

        var metrics = await service.GetMetricsAsync();

        // Assert
        metrics.Servicos.Should().HaveCount(2);
        
        var statsA = metrics.Servicos.First(s => s.Nome == "A");
        statsA.QuantidadeChamadas.Should().Be(2);
        statsA.MediaTempoRespostaMs.Should().Be(150);

        var statsB = metrics.Servicos.First(s => s.Nome == "B");
        statsB.QuantidadeChamadas.Should().Be(1);
        statsB.MediaTempoRespostaMs.Should().Be(300);
    }

    [Fact]
    public async Task GetMetricsAsync_ShouldReturnImmutableSnapshot()
    {
        // Arrange
        var service = new TelemetryService();
        service.RecordRequest("test", 100);

        // Act
        var metrics1 = await service.GetMetricsAsync();
        service.RecordRequest("test", 200);
        var metrics2 = await service.GetMetricsAsync();

        // Assert
        metrics1.Servicos.First().QuantidadeChamadas.Should().Be(1);
        metrics2.Servicos.First().QuantidadeChamadas.Should().Be(2);
    }
}