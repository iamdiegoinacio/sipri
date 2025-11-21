using MediatR;
using SIPRI.Application.DTOs.Simulacoes;

namespace SIPRI.Application.Queries.Simulacoes;

/// <summary>
/// Query para obter dados agregados das simulações (por produto e dia).
/// </summary>
public class GetSimulacoesAgregadasQuery : IRequest<IEnumerable<SimulacaoAgregadaDto>>
{
    public GetSimulacoesAgregadasQuery() { }
}
