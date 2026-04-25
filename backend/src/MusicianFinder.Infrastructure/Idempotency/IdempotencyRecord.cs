using MusicianFinder.SharedKernel;

namespace MusicianFinder.Infrastructure.Idempotency
{
    /// <summary>
    /// Запись идемпотентности для проверки повторных запросов.
    /// </summary>
    public class IdempotencyRecord : IInfrastructureEntity
    {
        /// <summary>Ключ идемпотентности.</summary>
        public string Key { get; set; } = default!;

        /// <summary>Хеш тела запроса.</summary>
        public string RequestHash { get; set; } = default!;

        /// <summary>Сериализованный ответ (заполняется после выполнения).</summary>
        public string? Response { get; set; }

        /// <summary>Статус выполнения: InProgress, Completed.</summary>
        public string Status { get; set; } = "InProgress";

        /// <summary>Дата создания записи.</summary>
        public DateTime CreatedAt { get; set; }
    }
}