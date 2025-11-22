using Microsoft.OpenApi.Models;

namespace SIPRI.Host.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSettings = configuration.GetSection("Keycloak");

        // 1. Captura as variáveis críticas para o Swagger UI
        var authorizationUrl = keycloakSettings["AuthorizationUrl"];
        var publicTokenUrl = keycloakSettings["PublicTokenUrl"];

        // 2. Validação Rigorosa (Fail Fast)
        // Verifica quais variáveis estão faltando para dar um erro preciso
        var missingVars = new List<string>();

        if (string.IsNullOrWhiteSpace(authorizationUrl))
            missingVars.Add("Keycloak:AuthorizationUrl");

        if (string.IsNullOrWhiteSpace(publicTokenUrl))
            missingVars.Add("Keycloak:PublicTokenUrl");

        if (missingVars.Any())
        {
            throw new InvalidOperationException(
                $"A inicialização do Swagger falhou devido à falta de configurações obrigatórias no appsettings ou variáveis de ambiente. " +
                $"Variáveis ausentes: {string.Join(", ", missingVars)}. " +
                $"O Swagger UI exige URLs acessíveis externamente (PublicTokenUrl) para funcionar corretamente via Browser.");
        }

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SIPRI API",
                Version = "v1",
                Description = "Sistema de Perfil de Risco Inteligente (Secured by Keycloak)"
            });

            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        // Usa as variáveis validadas acima
                        AuthorizationUrl = new Uri(authorizationUrl!),
                        TokenUrl = new Uri(publicTokenUrl!),
                        Scopes = new Dictionary<string, string> { }
                    }
                },
                Description = "Autenticação via Keycloak"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerWithAuthUI(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId("cli-web-sipri");
            options.OAuthAppName("SIPRI API");
            options.OAuthUsePkce();
            options.EnablePersistAuthorization();
        });

        return app;
    }
}