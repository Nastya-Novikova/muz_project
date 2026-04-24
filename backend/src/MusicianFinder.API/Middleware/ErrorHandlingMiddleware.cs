using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.API.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки исключений и возврата ProblemDetails (RFC 7807).
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
            var problem = new ProblemDetails
            {
                Instance = context.Request.Path,
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Внутренняя ошибка сервера",
                Type = "/errors/server"
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Title = "Ошибка валидации";
                    problem.Type = "/errors/validation";
                    problem.Extensions["errors"] = validationEx.Errors;
                    break;

                case DomainException domainEx:
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Title = domainEx.Message;
                    problem.Type = "/errors/domain";
                    break;

                case NotFoundException notFoundEx:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = notFoundEx.Message;
                    problem.Type = "/errors/not-found";
                    break;

                case ConflictException conflictEx:
                    problem.Status = (int)HttpStatusCode.Conflict;
                    problem.Title = conflictEx.Message;
                    problem.Type = "/errors/conflict";
                    break;

                case ForbiddenException forbiddenEx:
                    problem.Status = (int)HttpStatusCode.Forbidden;
                    problem.Title = forbiddenEx.Message;
                    problem.Type = "/errors/forbidden";
                    break;
            }

            context.Response.StatusCode = problem.Status ?? 500;
            context.Response.ContentType = "application/problem+json";
            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}