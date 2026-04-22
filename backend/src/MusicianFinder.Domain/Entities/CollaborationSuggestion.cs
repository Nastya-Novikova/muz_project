using System;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Предложение о сотрудничестве. Корень агрегата.
    /// </summary>
    public class CollaborationSuggestion : AggregateRoot
    {
        private CollaborationSuggestion()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр предложения.
        /// </summary>
        /// <param name="fromProfileId">Идентификатор отправителя.</param>
        /// <param name="toProfileId">Идентификатор получателя.</param>
        /// <param name="message">Сообщение.</param>
        public CollaborationSuggestion(Guid fromProfileId, Guid toProfileId, string? message = null)
        {
            Id = Guid.NewGuid();
            FromProfileId = fromProfileId;
            ToProfileId = toProfileId;
            Message = message;
            Status = SuggestionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Уникальный идентификатор предложения.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Идентификатор отправителя.
        /// </summary>
        public Guid FromProfileId { get; private set; }

        /// <summary>
        /// Идентификатор получателя.
        /// </summary>
        public Guid ToProfileId { get; private set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// Статус предложения.
        /// </summary>
        public SuggestionStatus Status { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Профиль отправителя.
        /// </summary>
        public MusicianProfile? FromProfile { get; private set; }

        /// <summary>
        /// Профиль получателя.
        /// </summary>
        public MusicianProfile? ToProfile { get; private set; }

        /// <summary>
        /// Принимает предложение.
        /// </summary>
        /// <exception cref="DomainException">Выбрасывается, если предложение не в статусе Pending.</exception>
        public void Accept()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Принять можно только ожидающее предложение.");

            Status = SuggestionStatus.Accepted;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отклоняет предложение.
        /// </summary>
        /// <exception cref="DomainException">Выбрасывается, если предложение не в статусе Pending.</exception>
        public void Reject()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Отклонить можно только ожидающее предложение.");

            Status = SuggestionStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отзывает предложение (отправителем).
        /// </summary>
        /// <exception cref="DomainException">Выбрасывается, если предложение не в статусе Pending.</exception>
        public void Withdraw()
        {
            if (Status != SuggestionStatus.Pending)
                throw new DomainException("Отозвать можно только ожидающее предложение.");

            Status = SuggestionStatus.Withdrawn;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}