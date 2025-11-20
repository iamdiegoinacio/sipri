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
        // Verifica se devemos ignorar esta rota ANTES de iniciar o cronômetro
        if (ShouldIgnore(context.Request.Path))
        {
            // Apenas repassa a requisição e sai. Não mede nada.
            await next(context);
            return;
        }

        // Se chegou aqui, é uma rota válida para medição
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

    /// <summary>
    /// Define quais rotas são consideradas "ruído" e não devem ser medidas.
    /// </summary>
    private static bool ShouldIgnore(PathString path)
    {
        if (!path.HasValue) return false;

        var p = path.Value.ToLowerInvariant();

        return p.StartsWith("/swagger") ||
               p.Contains("favicon") ||
               p.StartsWith("/telemetria");
    }
}