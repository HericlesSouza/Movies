using System.Net;
using System.Text.Json;

using FluentValidation;

namespace Movies.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode;
        object body;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;

                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                body = new
                {
                    title = "One or more validation errors occurred.",
                    status = (int)statusCode,
                    errors
                };
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;

                body = new
                {
                    title = "An unexpected error occurred.",
                    status = (int)statusCode
                };
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(body);
        await context.Response.WriteAsync(json);
    }
}
