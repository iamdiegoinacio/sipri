using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.UseCases.Perfis;

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

    /// <summary>
    /// Calcula e retorna o perfil de risco do cliente.
    /// </summary>
    /// <param name="clienteId">Identificador único do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Perfil de risco calculado.</returns>
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

    /// <summary>
    /// Recomenda produtos com base no perfil de risco.
    /// </summary>
    /// <param name="perfil">Perfil de risco do cliente (ex: Conservador, Moderado, Arrojado).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de produtos recomendados.</returns>
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