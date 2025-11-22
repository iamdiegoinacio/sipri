using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SIPRI.Application.Exceptions;
using SIPRI.Presentation.Middlewares;
using System.Text.Json;

namespace SIPRI.Presentation.Tests.Middlewares;

public class GlobalExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        var nextCalled = false;

        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ShouldHandleGenericException_AndReturnInternalServerError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) => throw new Exception("Unexpected error");

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Erro Interno do Servidor");
    }
}
