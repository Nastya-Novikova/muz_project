using System.Diagnostics;

namespace MusicianFinder.API.Middleware
{
    /// <summary>
    /// Middleware для логирования входящих запросов и времени выполнения.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RequestLoggingMiddleware"/>.
        /// </summary>
        /// <param name="next">Следующий делегат в конвейере.</param>
        /// <param name="logger">Логгер.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает HTTP-запрос.
        /// </summary>
        /// <param name="context">Контекст HTTP-запроса.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}