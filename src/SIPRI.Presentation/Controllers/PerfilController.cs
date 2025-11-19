using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.UseCases.Perfis;

namespace SIPRI.Presentation.Controllers;

[ApiController]
public class PerfilController : ControllerBase
{
    private readonly IMediator _mediator;

    public PerfilController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Calcula e retorna o perfil de risco do cliente.
    /// </summary>
    [HttpGet("perfil-risco/{clienteId}")]
    [ProducesResponseType(typeof(PerfilRiscoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPerfilRisco(Guid clienteId)
    {
        var query = new GetPerfilRiscoQuery(clienteId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Recomenda produtos com base no perfil de risco.
    /// </summary>
    [HttpGet("produtos-recomendados/{perfil}")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoRecomendadoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil)
    {
        var query = new GetProdutosRecomendadosQuery(perfil);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}