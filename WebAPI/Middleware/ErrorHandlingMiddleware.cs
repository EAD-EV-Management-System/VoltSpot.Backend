using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                FluentValidation.ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    validationEx.Message
                ),
                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    exception.Message
                ),
                InvalidOperationException => (
                    (int)HttpStatusCode.BadRequest,
                    exception.Message
                ),
                ArgumentException => (
                    (int)HttpStatusCode.BadRequest,
                    exception.Message
                ),
                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    "Resource not found"
                ),
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "An internal server error occurred"
                )
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                Success = false,
                Message = message
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
