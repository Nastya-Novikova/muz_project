using System.Net;
using System.Text.Json;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Domain.Exceptions;
using MusicianFinder.Shared.Models;

namespace MusicianFinder.API.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки исключений.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ExceptionHandlingMiddleware"/>.
        /// </summary>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает HTTP-запрос.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorCode, message, details) = exception switch
            {
                ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    "VALIDATION_ERROR",
                    "Ошибка валидации",
                    (object?)validationEx.Errors),

                NotFoundException notFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    "NOT_FOUND",
                    notFoundEx.Message,
                    null),

                ConflictException conflictEx => (
                    (int)HttpStatusCode.Conflict,
                    "CONFLICT",
                    conflictEx.Message,
                    null),

                ForbiddenException forbiddenEx => (
                    (int)HttpStatusCode.Forbidden,
                    "FORBIDDEN",
                    forbiddenEx.Message,
                    null),

                DomainException domainEx => (
                    (int)HttpStatusCode.BadRequest,
                    "DOMAIN_ERROR",
                    domainEx.Message,
                    null),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "INTERNAL_ERROR",
                    "Внутренняя ошибка сервера",
                    null)
            };

            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse(errorCode, message, details);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}