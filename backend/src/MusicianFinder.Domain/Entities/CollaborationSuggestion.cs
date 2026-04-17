using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Предложение о сотрудничестве.
    /// </summary>
    public class CollaborationSuggestion
    {
        private CollaborationSuggestion() { }

        public CollaborationSuggestion(Guid fromProfileId, Guid toProfileId, string? message = null)
        {
            Id = Guid.NewGuid();
            FromProfileId = fromProfileId;
            ToProfileId = toProfileId;
            Message = message;
            Status = "pending";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор предложения.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// ID отправителя.
        /// </summary>
        public Guid FromProfileId { get; private set; }

        /// <summary>
        /// ID получателя.
        /// </summary>
        public Guid ToProfileId { get; private set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// Статус: pending, accepted, rejected, withdrawn.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        // Навигационные свойства
        public MusicianProfile? FromProfile { get; private set; }
        public MusicianProfile? ToProfile { get; private set; }

        public void Accept()
        {
            if (Status != "pending")
                throw new InvalidOperationException("Only pending suggestions can be accepted.");
            Status = "accepted";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject()
        {
            if (Status != "pending")
                throw new InvalidOperationException("Only pending suggestions can be rejected.");
            Status = "rejected";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Withdraw()
        {
            if (Status != "pending")
                throw new InvalidOperationException("Only pending suggestions can be withdrawn.");
            Status = "withdrawn";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
