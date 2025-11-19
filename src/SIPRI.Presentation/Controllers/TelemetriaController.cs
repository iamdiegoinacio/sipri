using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.UseCases.Telemetria;
using Microsoft.AspNetCore.Http;

namespace SIPRI.Presentation.Controllers;

[ApiController]
[Route("telemetria")]
public class TelemetriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelemetriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Exibe métricas de uso e performance da API.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TelemetriaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTelemetria()
    {
        var query = new GetTelemetriaQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}