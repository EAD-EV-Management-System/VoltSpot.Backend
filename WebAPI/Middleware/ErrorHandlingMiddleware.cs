using System.Net;
using System.Text.Json;
using FluentValidation;

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

            // Handle FluentValidation.ValidationException with structured errors
            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Build field-specific error dictionary
                var errors = new Dictionary<string, string>();
                foreach (var error in validationEx.Errors)
                {
                    if (!errors.ContainsKey(error.PropertyName))
                    {
                        errors[error.PropertyName] = error.ErrorMessage;
                    }
                }

                var response = new
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            // Handle other exceptions
            var (statusCode, message) = exception switch
            {
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

            var standardResponse = new
            {
                Success = false,
                Message = message
            };

            var standardJsonResponse = JsonSerializer.Serialize(standardResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(standardJsonResponse);
        }
    }
}
