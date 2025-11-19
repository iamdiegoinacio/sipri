using MediatR;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.Interfaces;

namespace SIPRI.Application.UseCases.Telemetria;

/// <summary>
/// Query para consultar a saúde e performance da API.
/// </summary>
public class GetTelemetriaQuery : IRequest<TelemetriaDto>
{
    public GetTelemetriaQuery() { }
}

public class GetTelemetriaHandler : IRequestHandler<GetTelemetriaQuery, TelemetriaDto>
{
    private readonly ITelemetryService _telemetryService;

    public GetTelemetriaHandler(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    public async Task<TelemetriaDto> Handle(GetTelemetriaQuery request, CancellationToken cancellationToken)
    {
        // Delega a coleta de métricas para o serviço de infraestrutura
        return await _telemetryService.GetMetricsAsync();
    }
}