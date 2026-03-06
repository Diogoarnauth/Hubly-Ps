using System.Net;
using System.Text.Json;
using Hubly.api.Problems;

namespace Hubly.api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = ProblemResponse.InternalServerError;

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
