using Microsoft.OpenApi.Models;

namespace SIPRI.Host.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSettings = configuration.GetSection("Keycloak");

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
                        AuthorizationUrl = new Uri(keycloakSettings["AuthorizationUrl"]!),
                        TokenUrl = new Uri(keycloakSettings["TokenUrl"]!),
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