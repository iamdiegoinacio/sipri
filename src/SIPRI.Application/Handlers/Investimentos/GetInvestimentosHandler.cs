using MediatR;
using SIPRI.Application.Queries.Investimentos;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Handlers.Investimentos;

/// <summary>
/// Handler que busca os investimentos no repositório e mapeia para DTO.
/// </summary>
public class GetInvestimentosHandler : IRequestHandler<GetInvestimentosQuery, IEnumerable<HistoricoInvestimentoDto>>
{
    private readonly IInvestimentoRepository _investimentoRepository;

    public GetInvestimentosHandler(IInvestimentoRepository investimentoRepository)
    {
        _investimentoRepository = investimentoRepository;
    }

    public async Task<IEnumerable<HistoricoInvestimentoDto>> Handle(GetInvestimentosQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar dados no repositório
        var investimentos = await _investimentoRepository.GetByClienteIdAsync(request.ClienteId);

        // 2. Mapear Entidade -> DTO
        return investimentos.Select(i => new HistoricoInvestimentoDto
        {
            Id = i.Id,
            Tipo = i.Tipo,
            Valor = i.Valor,
            Rentabilidade = i.Rentabilidade,
            Data = i.Data
        });
    }
}
