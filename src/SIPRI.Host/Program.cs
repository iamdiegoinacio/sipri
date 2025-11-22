using Microsoft.EntityFrameworkCore;
using SIPRI.Application;
using SIPRI.Host.Extensions;
using SIPRI.Infrastructure;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Seed;
using SIPRI.Presentation.Controllers;
using SIPRI.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Configuração de Serviços (DI)
// ==========================================

// Conecta as camadas (Application e Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Adiciona o suporte a Controllers e avisa onde encontrá-los (na Presentation)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(SimulacaoController).Assembly);

builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger encapsulada no Host (Extension Method)
builder.Services.AddSwaggerWithAuth(builder.Configuration);

// Registra Middlewares Customizados na DI
builder.Services.AddScoped<GlobalExceptionHandlingMiddleware>();
builder.Services.AddScoped<TelemetryMiddleware>();

var app = builder.Build();

// ==========================================
// 2. Pipeline de Requisição HTTP
// ==========================================

// Middleware de Tratamento de Erros
app.UseGlobalExceptionHandler();

// Middleware de Telemetria
app.UseTelemetry();

if (app.Environment.IsDevelopment())
{
    // Configuração da UI do Swagger encapsulada
    app.UseSwaggerWithAuthUI(app.Configuration);
}

app.UseHttpsRedirection();

// Ordem de Segurança: AuthN antes de AuthZ
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os controllers para as rotas
app.MapControllers();

// ==========================================
// 3. Inicialização do Banco de Dados (Seed/Migrate)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Aplica migrações pendentes e cria o banco se não existir
        dbContext.Database.Migrate();

        // Popula o banco com dados iniciais
        DbInitializer.Seed(dbContext);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao inicializar o banco de dados.");
    }
}

app.Run();