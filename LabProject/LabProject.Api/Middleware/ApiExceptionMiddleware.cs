using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Middleware;

/// <summary>
/// Converts common application/domain exceptions into consistent HTTP responses.
/// This keeps the frontend from receiving generic 500 errors for validation issues.
/// </summary>
public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected / request aborted.
            context.Response.StatusCode = 499; // Client Closed Request (non-standard but common)
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Default: 500
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var title = "Error interno";
        var detail = "Ocurrió un error inesperado.";

        switch (ex)
        {
            case ArgumentNullException or ArgumentException:
                statusCode = (int)HttpStatusCode.BadRequest;
                title = "Solicitud inválida";
                detail = ex.Message;
                break;

            case InvalidOperationException:
                // Business rule / validation failure
                statusCode = (int)HttpStatusCode.BadRequest;
                title = "Validación";
                detail = ex.Message;
                break;

            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                title = "No encontrado";
                detail = ex.Message;
                break;
        }

        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
