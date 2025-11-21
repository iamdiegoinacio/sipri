using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.Queries.Perfis;

namespace SIPRI.Presentation.Controllers;

[ApiController]
[Authorize]
public class PerfilController : ControllerBase
{
    private readonly IMediator _mediator;

    public PerfilController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("perfil-risco/{clienteId}")]
    [ProducesResponseType(typeof(PerfilRiscoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPerfilRisco(Guid clienteId, CancellationToken cancellationToken)
    {
        var query = new GetPerfilRiscoQuery(clienteId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("produtos-recomendados/{perfil}")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoRecomendadoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil, CancellationToken cancellationToken)
    {
        var query = new GetProdutosRecomendadosQuery(perfil);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
