using SIPRI.Application;
using SIPRI.Infrastructure;
using SIPRI.Presentation.Middlewares;
using SIPRI.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using SIPRI.Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Configuração de Serviços (DI)
// ==========================================

//Conecta as camadas (Application e Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Adiciona o suporte a Controllers e avisa onde encontrá-los
builder.Services.AddControllers()
    .AddApplicationPart(typeof(SimulacaoController).Assembly);

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SIPRI API",
        Version = "v1",
        Description = "Sistema de Perfil de Risco Inteligente"
    });
});

// Registra Middlewares Customizados na DI
builder.Services.AddScoped<GlobalExceptionHandlingMiddleware>();
builder.Services.AddScoped<TelemetryMiddleware>();

var app = builder.Build();

// ==========================================
// 2. Pipeline de Requisição HTTP
// ==========================================

// Middleware de Tratamento de Erros
app.UseGlobalExceptionHandler();

// Middleware de Telemetria (medir tempo real)
app.UseTelemetry();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia os controllers para as rotas
app.MapControllers();

// ==========================================
// 3. Inicialização do Banco de Dados (Seed/Migrate)
// ==========================================

// Cria o escopo para rodar as migrations automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    // Resolve o DbContext do container de DI
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Aplica migrações pendentes e cria o banco se não existir
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
    }
}

app.Run();