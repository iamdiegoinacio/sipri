using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SIPRI.Application.Interfaces;
using SIPRI.Presentation.Middlewares;

namespace SIPRI.Presentation.Tests.Middlewares;

public class TelemetryMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldRecordRequest_WhenPathIsValid()
    {
        // Arrange
        var telemetryServiceMock = new Mock<ITelemetryService>();
        var middleware = new TelemetryMiddleware(telemetryServiceMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        
        RequestDelegate next = (ctx) => Task.Delay(10); // Simulate work

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        telemetryServiceMock.Verify(s => s.RecordRequest(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotRecordRequest_WhenPathIsIgnored()
    {
        // Arrange
        var telemetryServiceMock = new Mock<ITelemetryService>();
        var middleware = new TelemetryMiddleware(telemetryServiceMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/telemetria"; // Ignored path

        RequestDelegate next = (ctx) => Task.CompletedTask;

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        telemetryServiceMock.Verify(s => s.RecordRequest(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
    }
}
