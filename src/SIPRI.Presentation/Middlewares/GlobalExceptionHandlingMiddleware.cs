using SIPRI.Application.Exceptions; 
using SIPRI.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SIPRI.Presentation.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    // Injeta o Logger para logar o Erro 500 (o seu "Ponto 7")
    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // Tenta executar o próximo middleware no pipeline
            await next(context);
        }
        catch (Exception ex)
        {
            // Se uma exceção for capturada, chama nosso manipulador
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        // --- O "Tradutor" (Mapeia Exceção -> StatusCode) ---
        var (statusCode, title, detail) = exception switch
        {
            // Seus erros 4xx (Cliente)
            ValidationException e => (StatusCodes.Status400BadRequest, "Erro de Validação", e.Message),
            DomainRuleException e => (StatusCodes.Status400BadRequest, "Regra de Negócio Violada", e.Message),
            ForbiddenAccessException e => (StatusCodes.Status403Forbidden, "Acesso Negado", e.Message),
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso Não Encontrado", e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflito de Recurso", e.Message),

            // Erro 5xx (Servidor Externo)
            InfrastructureException e => (StatusCodes.Status503ServiceUnavailable, "Serviço Indisponível", e.Message),

            // --- O "Catch-All"---
            _ => (StatusCodes.Status500InternalServerError, "Erro Interno Inesperado", "Ocorreu um erro inesperado. Tente novamente.")
        };

        context.Response.StatusCode = statusCode;

        // Loga o erro 500 como CRÍTICO (o único que é um bug)
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro interno inesperado capturado pelo middleware: {Message}", exception.Message);
        }

        // Formata a resposta JSON
        var response = new
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            // Se for validação, retorna os detalhes
            Errors = exception is ValidationException vex ? vex.Errors : null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}