using LaboratorioAPI.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LaboratorioAPI.Middleware
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public GlobalExceptionHandlingMiddleware(
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment environment,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _logger = logger;
            _environment = environment;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request");
                await WriteProblemDetailsAsync(context, ex);
            }
        }

        private async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                DomainValidationException => StatusCodes.Status400BadRequest,
                ReservationConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode,
                title: "Se produjo un error al procesar la solicitud",
                detail: exception.Message,
                instance: context.Request.Path
            );

            if (statusCode == StatusCodes.Status500InternalServerError && _environment.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
