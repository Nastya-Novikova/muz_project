using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MusicianFinder.Application.Common.Behaviors
{
    /// <summary>
    /// Поведение MediatR, выполняющее логирование времени выполнения запроса.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LoggingBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Начало обработки запроса {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("Запрос {RequestName} обработан за {ElapsedMilliseconds} мс", requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
    }
}