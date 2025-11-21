using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Application.UseCases.Investimentos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace SIPRI.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("investimentos")]
public class InvestimentoController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestimentoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém o histórico de investimentos (carteira) do cliente.
    /// </summary>
    /// <param name="clienteId">Identificador único do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de investimentos do cliente.</returns>
    [HttpGet("{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoInvestimentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInvestimentos(Guid clienteId, CancellationToken cancellationToken)
    {
        var query = new GetInvestimentosQuery(clienteId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}