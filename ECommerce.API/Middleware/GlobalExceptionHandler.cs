using ECommerce.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            var (status, title) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                ConcurrencyException => (StatusCodes.Status409Conflict, "The data was modified by another request. Please try again."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            if (status == 500)
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
            else
                _logger.LogWarning("Handled exception ({Type}) on {Path}: {Message}",
                    exception.GetType().Name, context.Request.Path, exception.Message);

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Instance = context.Request.Path
            }, ct);

            return true;
        }


    }
}
