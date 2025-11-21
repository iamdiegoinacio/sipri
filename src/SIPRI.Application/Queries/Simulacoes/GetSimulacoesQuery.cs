using MediatR;
using SIPRI.Application.DTOs.Simulacoes;

namespace SIPRI.Application.Queries.Simulacoes;

/// <summary>
/// Query para listar o histórico de simulações realizadas por um cliente específico.
/// </summary>
public class GetSimulacoesQuery : IRequest<IEnumerable<HistoricoSimulacaoDto>>
{
    public Guid ClienteId { get; }

    public GetSimulacoesQuery(Guid clienteId)
    {
        ClienteId = clienteId;
    }
}
