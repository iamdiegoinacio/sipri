using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.Services;

namespace SIPRI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Registra o MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // ============================================================
        // 2. REGISTRO DOS SERVIÇOS DE DOMÍNIO (A CORREÇÃO)
        // ============================================================

        // Serviços Principais (Orquestradores do Domínio)
        services.AddScoped<ICalculadoraInvestimentoService, CalculadoraInvestimentoService>();
        services.AddScoped<IMotorPerfilRiscoServico, MotorPerfilRiscoServico>();

        // Estratégias de Cálculo (Strategy Pattern)
        // Registramos todas as implementações para a mesma interface
        services.AddScoped<IRegraCalculoInvestimento, RegraCalculoCDB>();
        services.AddScoped<IRegraCalculoInvestimento, RegraCalculoFundo>();

        // Estratégias de Pontuação de Risco
        services.AddScoped<IRegraDePontuacao, RegraPontuacaoFrequencia>();
        services.AddScoped<IRegraDePontuacao, RegraPontuacaoPreferencia>();
        services.AddScoped<IRegraDePontuacao, RegraPontuacaoVolume>();

        return services;
    }
}