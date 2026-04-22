using System.Net;
using System.Text.Json;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.API.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки исключений и возврата унифицированного ответа в формате ProblemDetails.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ErrorHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">Следующий делегат в конвейере.</param>
        /// <param name="logger">Логгер.</param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанное исключение");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/problem+json";

            var problem = new ApiProblemDetails
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problem.Type = "/errors/validation";
                    problem.Title = "Ошибка валидации";
                    problem.Status = response.StatusCode;
                    problem.Errors = validationEx.Errors;
                    break;

                case DomainException domainEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problem.Type = "/errors/domain";
                    problem.Title = "Нарушение бизнес-правил";
                    problem.Status = response.StatusCode;
                    problem.Errors = new Dictionary<string, string[]> { { "domain", new[] { domainEx.Message } } };
                    break;

                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    problem.Type = "/errors/not-found";
                    problem.Title = notFoundEx.Message;
                    problem.Status = response.StatusCode;
                    break;

                case ConflictException conflictEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    problem.Type = "/errors/conflict";
                    problem.Title = conflictEx.Message;
                    problem.Status = response.StatusCode;
                    break;

                case ForbiddenException forbiddenEx:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    problem.Type = "/errors/forbidden";
                    problem.Title = forbiddenEx.Message;
                    problem.Status = response.StatusCode;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problem.Type = "/errors/server";
                    problem.Title = "Внутренняя ошибка сервера";
                    problem.Status = response.StatusCode;
                    break;
            }

            var json = JsonSerializer.Serialize(problem);
            await response.WriteAsync(json);
        }

        private class ApiProblemDetails
        {
            public string Type { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public int Status { get; set; }
            public string TraceId { get; set; } = string.Empty;
            public IDictionary<string, string[]>? Errors { get; set; }
        }
    }
}