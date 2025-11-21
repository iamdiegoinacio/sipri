using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Simulacoes; 
using SIPRI.Application.Commands.Simulacoes;
using SIPRI.Application.Queries.Simulacoes;

namespace SIPRI.Presentation.Controllers;

[ApiController]
[Authorize]
public class SimulacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SimulacaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("simular-investimento")]
    [ProducesResponseType(typeof(SimulacaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SimularInvestimento([FromBody] SimulacaoRequestDto request, CancellationToken cancellationToken)
    {
        var command = new SimularInvestimentoCommand(request);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("simulacoes")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoSimulacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSimulacoes([FromQuery] Guid clienteId, CancellationToken cancellationToken)
    {
        var query = new GetSimulacoesQuery(clienteId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("simulacoes/por-produto-dia")]
    [ProducesResponseType(typeof(IEnumerable<SimulacaoAgregadaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSimulacoesAgregadas(CancellationToken cancellationToken)
    {
        var query = new GetSimulacoesAgregadasQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
