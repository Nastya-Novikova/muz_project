using backend.Exceptions;
using System.Text.Json;

namespace backend.Middleware
{
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
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message, errorCode) = exception switch
            {
                ApiException apiEx => (apiEx.StatusCode, apiEx.Message, apiEx.ErrorCode),
                _ => (500, "Внутренняя ошибка сервера", "INTERNAL_ERROR")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = new
                {
                    code = errorCode,
                    message,
                    timestamp = DateTime.UtcNow
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
