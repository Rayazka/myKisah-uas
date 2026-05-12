using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using myKisah.Utils;

namespace myKisah.Tests.ValidationTests;

public class MiddlewareTests
{
    [Fact]
    public async Task Middleware_ArgumentException_Returns400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new ArgumentException("Test error");
        var middleware = new ErrorHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("error", responseBody);
        Assert.Contains("400", responseBody);
    }

    [Fact]
    public async Task Middleware_KeyNotFoundException_Returns404()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new KeyNotFoundException("not found");
        var middleware = new ErrorHandlingMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_InvalidOperationException_Returns422()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new InvalidOperationException("invalid");
        var middleware = new ErrorHandlingMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.Equal(422, context.Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_UnknownException_Returns500()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new Exception("unknown error");
        var middleware = new ErrorHandlingMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.DoesNotContain("unknown error", responseBody); // pastikan internal message tidak bocor
    }
}