using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        // 1. Configuração do Entity Framework Core (SQL Server)
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // 2. Registro dos Repositórios (Scoped - tempo de vida do request)
        services.AddScoped<IProdutoInvestimentoRepository, ProdutoInvestimentoRepository>();
        services.AddScoped<IInvestimentoRepository, InvestimentoRepository>();
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Registro dos Serviços de Infra (Singleton ou Scoped)
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Telemetria
        services.AddSingleton<TelemetryService>();
        services.AddSingleton<ITelemetryService>(provider => provider.GetRequiredService<TelemetryService>());

        return services;
    }
}