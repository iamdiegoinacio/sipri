using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SIPRI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra o MediatR e varre o assembly atual (Application)
        // procurando por todos os Commands, Queries e Handlers para registrar automaticamente.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}