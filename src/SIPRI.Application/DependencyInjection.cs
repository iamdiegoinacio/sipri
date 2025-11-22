using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.Services;
using FluentValidation;
using MediatR;
using SIPRI.Application.Behaviors;

namespace SIPRI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Registra o MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // 2. Registra FluentValidation
        // Escaneia o assembly atual e registra todos os validadores
        services.AddValidatorsFromAssembly(assembly);

        // 3. Registra ValidationBehavior como Pipeline Behavior do MediatR
        // Isso faz com que todas as requests passem pelo ValidationBehavior antes do handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // ============================================================
        // 4. REGISTRO DOS SERVIÇOS DE DOMÍNIO
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
