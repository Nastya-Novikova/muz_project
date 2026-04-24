namespace MusicianFinder.Infrastructure.Outbox
{
    /// <summary>
    /// Сообщение Dead Letter Queue — событие, которое не удалось обработать после нескольких попыток.
    /// </summary>
    public class DeadLetter
    {
        /// <summary>Идентификатор записи.</summary>
        public Guid Id { get; set; }

        /// <summary>Идентификатор исходного сообщения Outbox.</summary>
        public Guid OutboxMessageId { get; set; }

        /// <summary>Ошибка, из-за которой сообщение попало в DLQ.</summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>Дата перемещения.</summary>
        public DateTime MovedAt { get; set; }
    }
}