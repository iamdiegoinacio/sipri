using SIPRI.Application.DTOs.Telemetria;

namespace SIPRI.Application.Interfaces;

public interface ITelemetryService
{
    // Leitura (usado pelo Handler)
    Task<TelemetriaDto> GetMetricsAsync();

    // Gravação (usado pelo Middleware)
    void RecordRequest(string endpointName, long elapsedMs);
}
