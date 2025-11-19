using MediatR;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.UseCases.Simulacoes;

/// <summary>
/// Query para obter dados agregados das simulações (por produto e dia).
/// </summary>
public class GetSimulacoesAgregadasQuery : IRequest<IEnumerable<SimulacaoAgregadaDto>>
{
    public GetSimulacoesAgregadasQuery() { }
}

public class GetSimulacoesAgregadasHandler : IRequestHandler<GetSimulacoesAgregadasQuery, IEnumerable<SimulacaoAgregadaDto>>
{
    private readonly ISimulacaoRepository _simulacaoRepository;

    public GetSimulacoesAgregadasHandler(ISimulacaoRepository simulacaoRepository)
    {
        _simulacaoRepository = simulacaoRepository;
    }

    public async Task<IEnumerable<SimulacaoAgregadaDto>> Handle(GetSimulacoesAgregadasQuery request, CancellationToken cancellationToken)
    {
        // 1. Busca os dados já agregados pelo repositório
        var dadosAgregados = await _simulacaoRepository.GetAgregadoPorDiaAsync();

        // 2. Mapeia para o DTO
        return dadosAgregados.Select(d => new SimulacaoAgregadaDto
        {
            Produto = d.Produto,
            Data = DateOnly.FromDateTime(d.Data),
            QuantidadeSimulacoes = d.QuantidadeSimulacoes,
            MediaValorFinal = d.MediaValorFinal
        });
    }
}