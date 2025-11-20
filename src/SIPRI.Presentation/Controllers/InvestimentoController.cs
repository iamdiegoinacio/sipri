using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Application.UseCases.Investimentos;
using Microsoft.AspNetCore.Http;

namespace SIPRI.Presentation.Controllers;

[ApiController]
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
    [HttpGet("{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoInvestimentoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvestimentos(Guid clienteId)
    {
        var query = new GetInvestimentosQuery(clienteId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}