using System.Diagnostics;
using SIPRI.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SIPRI.Presentation.Middlewares;

public class TelemetryMiddleware : IMiddleware
{
    private readonly ITelemetryService _telemetryService;

    public TelemetryMiddleware(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var endpointName = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? "Unknown";

            _telemetryService.RecordRequest(endpointName, stopwatch.ElapsedMilliseconds);
        }
    }
}