using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SIPRI.Application.Interfaces;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;
using SIPRI.Infrastructure.Services;

namespace SIPRI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. SQL Server
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // 2. Repositórios
        services.AddScoped<IProdutoInvestimentoRepository, ProdutoInvestimentoRepository>();
        services.AddScoped<IInvestimentoRepository, InvestimentoRepository>();
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Serviços
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITelemetryService, TelemetryService>();

        // Limpa o mapeamento padrão de claims da Microsoft para manter os nomes originais (sub, preferred_username, etc)
        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // 4. Autenticação com Tratamento Inteligente de Erros
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            var authConfig = configuration.GetSection("Authentication");
            var audience = authConfig["Audience"];
            var authority = authConfig["Authority"];

            // Validação de Configuração Crítica (Fail Fast)
            if (string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(authority))
            {
                throw new InvalidOperationException("Configuração crítica de autenticação ausente. Verifique 'Authentication:Audience' e 'Authentication:Authority' no appsettings.json.");
            }

            o.Authority = authority;
            o.Audience = audience;
            o.RequireHttpsMetadata = false; // Ambiente de Dev/Docker sem SSL externo

            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig["ValidIssuer"] ?? authority,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero // Remove a tolerância padrão de 5min para expiração
            };

            // Eventos para tratamento avançado de erros e logging
            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SIPRI.Infrastructure.Auth");

                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        logger.LogWarning("Autenticação falhou: Token expirado.");

                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    else
                    {
                        logger.LogError(context.Exception, "Autenticação falhou: Token inválido ou erro de validação.");
                    }

                    return Task.CompletedTask;
                },

                OnForbidden = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SIPRI.Infrastructure.Auth");

                    logger.LogWarning("Acesso proibido (403): Usuário autenticado, mas sem permissão para este recurso.");
                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SIPRI.Infrastructure.Auth");

                    logger.LogDebug("Token validado com sucesso para Audience: {Audience}", context.Principal?.FindFirst("aud")?.Value);
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}