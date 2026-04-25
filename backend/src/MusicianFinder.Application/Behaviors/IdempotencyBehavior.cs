using System.Text.Json;
using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Behaviors
{
    /// <summary>
    /// Поведение MediatR, обеспечивающее идемпотентность команд.
    /// Проверяет наличие ранее выполненной команды с таким же ключом идемпотентности
    /// и либо возвращает сохранённый ответ, либо выполняет команду и сохраняет ответ.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IIdempotencyStore _store;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IdempotencyBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="store">Хранилище записей идемпотентности.</param>
        public IdempotencyBehavior(IIdempotencyStore store)
        {
            _store = store;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.IdempotencyKey))
                return await next();

            var key = request.IdempotencyKey;
            var requestHash = ComputeHash(request);

            var (created, record) = await _store.TryCreateAsync(key, requestHash);

            if (created)
            {
                var response = await next();
                var serializedResponse = JsonSerializer.Serialize(response);
                await _store.UpdateAsync(key, serializedResponse, "Completed");
                return response;
            }

            if (record!.RequestHash != requestHash)
                throw new IdempotencyConflictException("Несовпадение хеша запроса – ключ идемпотентности уже использован для другого запроса.");

            if (record.Status == "InProgress")
                throw new IdempotencyConflictException("Запрос с этим ключом уже выполняется.");

            return JsonSerializer.Deserialize<TResponse>(record.Response!)!;
        }

        /// <summary>
        /// Вычисляет хеш запроса для сравнения.
        /// </summary>
        private static string ComputeHash(TRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(bytes);
        }
    }
}