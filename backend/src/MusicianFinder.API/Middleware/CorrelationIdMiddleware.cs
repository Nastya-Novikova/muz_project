using Microsoft.Extensions.Primitives;

namespace MusicianFinder.API.Middleware
{
    /// <summary>
    /// Middleware для добавления или передачи идентификатора корреляции.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CorrelationIdMiddleware"/>.
        /// </summary>
        /// <param name="next">Следующий делегат в конвейере.</param>
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Обрабатывает HTTP-запрос.
        /// </summary>
        /// <param name="context">Контекст HTTP-запроса.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
                context.Request.Headers.Append("X-Correlation-Id", correlationId);
            }

            context.Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
            await _next(context);
        }
    }
}