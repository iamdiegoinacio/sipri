using SIPRI.Application.DTOs.Telemetria;

namespace SIPRI.Application.Interfaces;

/// <summary>
/// Interface para obter métricas de performance do sistema.
/// A implementação (Infra) lerá os contadores ou logs.
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Retorna o snapshot atual das métricas de telemetria.
    /// </summary>
    Task<TelemetriaDto> GetMetricsAsync();
}