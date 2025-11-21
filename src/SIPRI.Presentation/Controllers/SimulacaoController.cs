using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Simulacoes; 
using SIPRI.Application.UseCases.Simulacoes;

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

    /// <summary>
    /// Solicita uma nova simulação de investimento.
    /// </summary>
    /// <param name="request">Dados da simulação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Resultado da simulação.</returns>
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

    /// <summary>
    /// Obtém o histórico de simulações de um cliente.
    /// </summary>
    /// <param name="clienteId">Identificador único do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Histórico de simulações.</returns>
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

    /// <summary>
    /// Obtém dados agregados das simulações por produto e dia.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Dados agregados das simulações.</returns>
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