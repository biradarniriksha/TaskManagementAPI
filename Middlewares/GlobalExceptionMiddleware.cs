using System.Net;
using System.Text.Json;

namespace TaskManagementAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // continue pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }

            // Also handle status codes manually like 401, 403 after _next
            if (!context.Response.HasStarted &&
                (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                 context.Response.StatusCode == (int)HttpStatusCode.Forbidden))
            {
                await HandleStatusCodeAsync(context);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Unauthorized access";
            }
            else if (exception is ApplicationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private Task HandleStatusCodeAsync(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;
            var message = statusCode switch
            {
                401 => "Unauthorized access",
                403 => "Forbidden access",
                _ => "An error occurred"
            };

            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
