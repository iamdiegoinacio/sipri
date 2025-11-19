using MediatR;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.UseCases.Perfis;

/// <summary>
/// Query para listar produtos de investimento adequados a um perfil de risco específico.
/// </summary>
public class GetProdutosRecomendadosQuery : IRequest<IEnumerable<ProdutoRecomendadoDto>>
{
    public string Perfil { get; }

    public GetProdutosRecomendadosQuery(string perfil)
    {
        Perfil = perfil;
    }
}

/// <summary>
/// Handler que busca os produtos filtrados pelo perfil no repositório.
/// </summary>
public class GetProdutosRecomendadosHandler : IRequestHandler<GetProdutosRecomendadosQuery, IEnumerable<ProdutoRecomendadoDto>>
{
    private readonly IProdutoInvestimentoRepository _produtoRepository;

    public GetProdutosRecomendadosHandler(IProdutoInvestimentoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<ProdutoRecomendadoDto>> Handle(GetProdutosRecomendadosQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar no repositório (Camada de Persistência)
        var produtos = await _produtoRepository.GetByPerfilRiscoAsync(request.Perfil);

        // 2. Mapeamento de Entidade -> DTO
        return produtos.Select(p => new ProdutoRecomendadoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Tipo = p.Tipo,
            Rentabilidade = p.RentabilidadeBase,
            Risco = p.Risco
        });
    }
}