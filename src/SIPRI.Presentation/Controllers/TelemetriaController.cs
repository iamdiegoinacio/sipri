using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.Queries.Telemetria;

namespace SIPRI.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("telemetria")]
public class TelemetriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelemetriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(TelemetriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTelemetria(CancellationToken cancellationToken)
    {
        var query = new GetTelemetriaQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
