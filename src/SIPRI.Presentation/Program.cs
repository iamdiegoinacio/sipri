using SIPRI.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Registra o Middleware na Injeção de Dependência (como Scoped)
builder.Services.AddScoped<GlobalExceptionHandlingMiddleware>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();