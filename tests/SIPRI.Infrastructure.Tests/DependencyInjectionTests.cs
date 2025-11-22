using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIPRI.Application.Interfaces;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Infrastructure;

namespace SIPRI.Infrastructure.Tests;

public class DependencyInjectionTests
{
    private IConfiguration CreateConfiguration()
    {
        var config = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDb;User=sa;Password=Test123" },
            { "Authentication:Authority", "https://test-authority.com" },
            { "Authentication:Audience", "test-audience" },
            { "Authentication:ValidIssuer", "https://test-issuer.com" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(config!)
            .Build();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterRepositories()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IProdutoInvestimentoRepository>().Should().NotBeNull();
        provider.GetService<IInvestimentoRepository>().Should().NotBeNull();
        provider.GetService<ISimulacaoRepository>().Should().NotBeNull();
        provider.GetService<IUnitOfWork>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IDateTimeProvider>().Should().NotBeNull();
        provider.GetService<ITelemetryService>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterSingletonServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        var dateProvider1 = provider.GetService<IDateTimeProvider>();
        var dateProvider2 = provider.GetService<IDateTimeProvider>();

        // Assert
        dateProvider1.Should().BeSameAs(dateProvider2);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterScopedRepositories()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var repo1 = scope1.ServiceProvider.GetService<IProdutoInvestimentoRepository>();
        var repo2 = scope2.ServiceProvider.GetService<IProdutoInvestimentoRepository>();

        // Assert
        repo1.Should().NotBeSameAs(repo2);
    }
}