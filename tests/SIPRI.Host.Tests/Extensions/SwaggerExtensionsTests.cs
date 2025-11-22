using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIPRI.Host.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

namespace SIPRI.Host.Tests.Extensions;

public class SwaggerExtensionsTests
{
    [Fact]
    public void AddSwaggerWithAuth_ShouldThrowInvalidOperationException_WhenAuthorizationUrlIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:PublicTokenUrl", "https://keycloak.example.com/token" }
            }!)
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Keycloak:AuthorizationUrl*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldThrowInvalidOperationException_WhenPublicTokenUrlIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "https://keycloak.example.com/auth" }
            }!)
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Keycloak:PublicTokenUrl*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldThrowInvalidOperationException_WhenBothUrlsAreMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Keycloak:AuthorizationUrl*")
            .WithMessage("*Keycloak:PublicTokenUrl*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldThrowInvalidOperationException_WhenAuthorizationUrlIsEmpty()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "" },
                { "Keycloak:PublicTokenUrl", "https://keycloak.example.com/token" }
            }!)
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Keycloak:AuthorizationUrl*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldThrowInvalidOperationException_WhenPublicTokenUrlIsWhitespace()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "https://keycloak.example.com/auth" },
                { "Keycloak:PublicTokenUrl", "   " }
            }!)
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Keycloak:PublicTokenUrl*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldRegisterSwaggerGen_WhenAllConfigurationsAreValid()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "https://keycloak.example.com/auth" },
                { "Keycloak:PublicTokenUrl", "https://keycloak.example.com/token" }
            }!)
            .Build();

        // Act
        var result = services.AddSwaggerWithAuth(configuration);

        // Assert
        result.Should().BeSameAs(services);
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        swaggerGenOptions.Should().NotBeNull();
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldReturnServiceCollection_WhenSuccessful()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "https://keycloak.example.com/auth" },
                { "Keycloak:PublicTokenUrl", "https://keycloak.example.com/token" }
            }!)
            .Build();

        // Act
        var result = services.AddSwaggerWithAuth(configuration);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ServiceCollection>();
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldIncludeCorrectErrorMessage_WhenConfigurationIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*A inicialização do Swagger falhou*")
            .WithMessage("*configurações obrigatórias*")
            .WithMessage("*Variáveis ausentes*");
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldValidateUrlFormat_WhenUrlsAreProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Keycloak:AuthorizationUrl", "https://keycloak.example.com/realms/sipri/protocol/openid-connect/auth" },
                { "Keycloak:PublicTokenUrl", "https://keycloak.example.com/realms/sipri/protocol/openid-connect/token" }
            }!)
            .Build();

        // Act
        Action act = () => services.AddSwaggerWithAuth(configuration);

        // Assert
        act.Should().NotThrow();
    }
}