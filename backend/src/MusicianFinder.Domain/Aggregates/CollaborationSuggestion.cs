using MusicianFinder.SharedKernel;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Предложение о сотрудничестве. Корень агрегата.
    /// </summary>
    public class CollaborationSuggestion : AggregateRoot
    {
        private CollaborationSuggestion()
        {
            // Поля инициализируются через публичные конструкторы
        }

        /// <summary>
        /// Инициализирует новое предложение.
        /// </summary>
        /// <param name="fromProfileId">Идентификатор отправителя.</param>
        /// <param name="toProfileId">Идентификатор получателя.</param>
        /// <param name="message">Текст сообщения (необязательно).</param>
        public CollaborationSuggestion(Guid fromProfileId, Guid toProfileId, string? message = null)
        {
            Id = Guid.NewGuid();
            FromProfileId = fromProfileId;
            ToProfileId = toProfileId;
            Message = message;
            Status = SuggestionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new CollaborationSuggestionSent(Id, fromProfileId, toProfileId));
        }

        /// <summary>Идентификатор отправителя.</summary>
        public Guid FromProfileId { get; private set; }

        /// <summary>Идентификатор получателя.</summary>
        public Guid ToProfileId { get; private set; }

        /// <summary>Текст сообщения.</summary>
        public string? Message { get; private set; }

        /// <summary>Статус предложения.</summary>
        public SuggestionStatus Status { get; private set; }

        /// <summary>Дата создания.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Дата последнего обновления.</summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Принимает предложение.
        /// </summary>
        public void Accept()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Принять можно только ожидающее предложение.");
            Status = SuggestionStatus.Accepted;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CollaborationSuggestionAccepted(Id));
        }

        /// <summary>
        /// Отклоняет предложение.
        /// </summary>
        public void Reject()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Отклонить можно только ожидающее предложение.");
            Status = SuggestionStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CollaborationSuggestionRejected(Id));
        }

        /// <summary>
        /// Отзывает предложение (отправителем).
        /// </summary>
        public void Withdraw()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Отозвать можно только ожидающее предложение.");
            Status = SuggestionStatus.Withdrawn;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}