using MediatR;
using SIPRI.Application.DTOs.Perfis;

namespace SIPRI.Application.Queries.Perfis;

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
