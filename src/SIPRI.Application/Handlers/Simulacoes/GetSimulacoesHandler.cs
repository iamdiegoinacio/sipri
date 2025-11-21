using MediatR;
using SIPRI.Application.Queries.Simulacoes;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.Handlers.Simulacoes;

public class GetSimulacoesHandler : IRequestHandler<GetSimulacoesQuery, IEnumerable<HistoricoSimulacaoDto>>
{
    private readonly ISimulacaoRepository _simulacaoRepository;

    public GetSimulacoesHandler(ISimulacaoRepository simulacaoRepository)
    {
        _simulacaoRepository = simulacaoRepository;
    }

    public async Task<IEnumerable<HistoricoSimulacaoDto>> Handle(GetSimulacoesQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar simulações filtradas pelo ClienteId
        var simulacoes = await _simulacaoRepository.GetByClienteIdAsync(request.ClienteId);

        // 2. Mapear Entidade -> DTO
        return simulacoes.Select(s => new HistoricoSimulacaoDto
        {
            Id = s.Id,
            ClienteId = s.ClienteId,
            Produto = s.ProdutoNome, // Nome desnormalizado
            ValorInvestido = s.ValorInvestido,
            ValorFinal = s.ValorFinal,
            PrazoMeses = s.PrazoMeses,
            DataSimulacao = s.DataSimulacao
        });
    }
}
