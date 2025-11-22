using MediatR;
using SIPRI.Application.DTOs.Investimentos;

namespace SIPRI.Application.Queries.Investimentos;

/// <summary>
/// Query para consultar o histórico de investimentos (carteira) de um cliente.
/// </summary>
public class GetInvestimentosQuery : IRequest<IEnumerable<HistoricoInvestimentoDto>>
{
    public Guid ClienteId { get; }

    public GetInvestimentosQuery(Guid clienteId)
    {
        ClienteId = clienteId;
    }
}
