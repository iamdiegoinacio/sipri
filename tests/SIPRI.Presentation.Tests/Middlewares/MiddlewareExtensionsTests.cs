using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SIPRI.Presentation.Middlewares;

namespace SIPRI.Presentation.Tests.Middlewares;

public class MiddlewareExtensionsTests
{
    private IApplicationBuilder CreateApplicationBuilder()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        return new ApplicationBuilder(serviceProvider);
    }

    [Fact]
    public void UseGlobalExceptionHandler_ShouldReturnApplicationBuilder()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act
        var result = app.UseGlobalExceptionHandler();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(app);
    }

    [Fact]
    public void UseGlobalExceptionHandler_ShouldRegisterMiddlewareInPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<GlobalExceptionHandlingMiddleware>();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseGlobalExceptionHandler();

        // Assert
        result.Should().NotBeNull();
        // O middleware deve estar registrado no pipeline
        var middleware = app.Build();
        middleware.Should().NotBeNull();
    }

    [Fact]
    public void UseTelemetry_ShouldReturnApplicationBuilder()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act
        var result = app.UseTelemetry();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(app);
    }

    [Fact]
    public void UseTelemetry_ShouldRegisterMiddlewareInPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var mockTelemetryService = new Mock<SIPRI.Application.Interfaces.ITelemetryService>();
        services.AddScoped(_ => mockTelemetryService.Object);
        services.AddScoped<TelemetryMiddleware>();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseTelemetry();

        // Assert
        result.Should().NotBeNull();
        var middleware = app.Build();
        middleware.Should().NotBeNull();
    }

    [Fact]
    public void MiddlewareExtensions_ShouldAllowBothMiddlewaresInPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app
            .UseGlobalExceptionHandler()
            .UseTelemetry()
            .UseRouting();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(app);
    }

    [Fact]
    public void UseGlobalExceptionHandler_ShouldNotThrowException_WhenCalledMultipleTimes()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act
        Action act = () =>
        {
            app.UseGlobalExceptionHandler();
            app.UseGlobalExceptionHandler();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UseTelemetry_ShouldNotThrowException_WhenCalledMultipleTimes()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act
        Action act = () =>
        {
            app.UseTelemetry();
            app.UseTelemetry();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UseGlobalExceptionHandler_ShouldWorkWithMinimalConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        Action act = () => app.UseGlobalExceptionHandler();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UseTelemetry_ShouldWorkWithMinimalConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        Action act = () => app.UseTelemetry();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UseGlobalExceptionHandler_ShouldBeExtensionMethod()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act & Assert
        // Verifica que o método é uma extensão de IApplicationBuilder
        var method = typeof(MiddlewareExtensions).GetMethod("UseGlobalExceptionHandler");
        method.Should().NotBeNull();
        method!.IsStatic.Should().BeTrue();
        method.GetParameters().Should().HaveCount(1);
        method.GetParameters()[0].ParameterType.Should().Be(typeof(IApplicationBuilder));
    }

    [Fact]
    public void UseTelemetry_ShouldBeExtensionMethod()
    {
        // Arrange
        var app = CreateApplicationBuilder();

        // Act & Assert
        // Verifica que o método é uma extensão de IApplicationBuilder
        var method = typeof(MiddlewareExtensions).GetMethod("UseTelemetry");
        method.Should().NotBeNull();
        method!.IsStatic.Should().BeTrue();
        method.GetParameters().Should().HaveCount(1);
        method.GetParameters()[0].ParameterType.Should().Be(typeof(IApplicationBuilder));
    }

    [Fact]
    public void MiddlewareExtensions_ClassShouldBeStaticAndSealed()
    {
        // Arrange & Act
        var type = typeof(MiddlewareExtensions);

        // Assert
        type.IsAbstract.Should().BeTrue("a classe deve ser static (abstract + sealed no IL)");
        type.IsSealed.Should().BeTrue("a classe deve ser static (abstract + sealed no IL)");
    }
}