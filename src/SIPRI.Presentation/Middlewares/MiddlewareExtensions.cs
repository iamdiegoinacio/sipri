using Microsoft.AspNetCore.Builder;

namespace SIPRI.Presentation.Middlewares;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware global de tratamento de exceções ao pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adiciona o middleware de telemetria ao pipeline.
    /// </summary>
    public static IApplicationBuilder UseTelemetry(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TelemetryMiddleware>();
    }
}