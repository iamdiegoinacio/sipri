using MediatR;
using SIPRI.Application.DTOs.Telemetria;

namespace SIPRI.Application.Queries.Telemetria;

/// <summary>
/// Query para consultar a saúde e performance da API.
/// </summary>
public class GetTelemetriaQuery : IRequest<TelemetriaDto>
{
    public GetTelemetriaQuery() { }
}
