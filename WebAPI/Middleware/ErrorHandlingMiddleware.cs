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

            var response = new
            {
                Success = false,
                Message = "An error occurred",
                Details = exception.Message
            };

            //switch (exception)
            //{
            //    case UnauthorizedAccessException:
            //        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //        response = new { Success = false, Message = exception.Message };
            //        break;

            //    case InvalidOperationException:
            //        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        response = new { Success = false, Message = exception.Message };
            //        break;

            //    case ArgumentException:
            //        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        response = new { Success = false, Message = exception.Message };
            //        break;

            //    case KeyNotFoundException:
            //        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            //        response = new { Success = false, Message = "Resource not found" };
            //        break;

            //    default:
            //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //        response = new { Success = false, Message = "An internal server error occurred" };
            //        break;
            //}

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
