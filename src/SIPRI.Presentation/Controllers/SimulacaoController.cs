using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Simulacoes; 
using SIPRI.Application.UseCases.Simulacoes;

namespace SIPRI.Presentation.Controllers;

[ApiController]
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
    [HttpPost("simular-investimento")]
    [ProducesResponseType(typeof(SimulacaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SimularInvestimento([FromBody] SimulacaoRequestDto request)
    {
        var command = new SimularInvestimentoCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Obtém o histórico de simulações de um cliente.
    /// </summary>
    [HttpGet("simulacoes")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoSimulacaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSimulacoes([FromQuery] Guid clienteId)
    {
        var query = new GetSimulacoesQuery(clienteId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém dados agregados das simulações por produto e dia.
    /// </summary>
    [HttpGet("simulacoes/por-produto-dia")]
    [ProducesResponseType(typeof(IEnumerable<SimulacaoAgregadaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSimulacoesAgregadas()
    {
        var query = new GetSimulacoesAgregadasQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}