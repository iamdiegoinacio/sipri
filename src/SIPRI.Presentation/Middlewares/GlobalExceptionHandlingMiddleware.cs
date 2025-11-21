using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIPRI.Application.Exceptions;
using SIPRI.Domain.Exceptions;
using System.Diagnostics;
using System.Text.Json;

namespace SIPRI.Presentation.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        // Mapeamento de Exceção para Status Code e Detalhes
        var (statusCode, title, detail) = exception switch
        {
            ValidationException e => (StatusCodes.Status400BadRequest, "Erro de Validação", e.Message),
            DomainRuleException e => (StatusCodes.Status400BadRequest, "Regra de Negócio Violada", e.Message),
            ForbiddenAccessException e => (StatusCodes.Status403Forbidden, "Acesso Negado", e.Message),
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso Não Encontrado", e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflito de Recurso", e.Message),
            InfrastructureException e => (StatusCodes.Status503ServiceUnavailable, "Serviço Indisponível", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", "Ocorreu um erro inesperado ao processar sua solicitação.")
        };

        context.Response.StatusCode = statusCode;

        // Loga erro 500 como Error, outros como Warning ou Information
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro crítico não tratado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Erro tratado ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        // Cria o objeto ProblemDetails (Padrão RFC 7807)
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // Adiciona TraceId para rastreabilidade
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions.Add("traceId", traceId);

        // Se for erro de validação, adiciona os erros específicos
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("errors", validationException.Errors);
        }

        // Serializa e escreve a resposta
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsJsonAsync(problemDetails, jsonOptions);
    }
}