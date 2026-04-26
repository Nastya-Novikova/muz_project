using Microsoft.EntityFrameworkCore.Infrastructure;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Infrastructure.Outbox
{
    /// <summary>
    /// Сообщение Outbox для гарантированной доставки интеграционных событий.
    /// </summary>
    public class OutboxMessage : IInfrastructureEntity
    {
        /// <summary>Идентификатор сообщения.</summary>
        public Guid Id { get; set; }

        /// <summary>Имя интеграционного события (например "profile.created").</summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>Версия формата события.</summary>
        public int Version { get; set; }

        /// <summary>Сериализованное тело события.</summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>Идентификатор корреляции.</summary>
        public string? CorrelationId { get; set; }

        /// <summary>Дата создания.</summary>
        public DateTime OccurredAt { get; set; }

        /// <summary>Дата обработки (если обработано).</summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>Дата следующей попытки.</summary>
        public DateTime NextAttemptAt { get; set; }

        /// <summary>Количество попыток.</summary>
        public int RetryCount { get; set; }

        /// <summary>Ошибка последней попытки.</summary>
        public string? Error { get; set; }
    }
}