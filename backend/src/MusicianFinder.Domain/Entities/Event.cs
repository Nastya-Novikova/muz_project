using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Мероприятие.
    /// </summary>
    public class Event : ISoftDeletable
    {
        private readonly List<EventRegistration> _registrations = new();

        private Event() { } // для EF Core

        public Event(
            string title,
            int regionId,
            int cityId,
            string address,
            DateTime startDateTime,
            Guid creatorProfileId,
            string? description = null,
            DateTime? endDateTime = null,
            int maxParticipants = 0)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description;
            RegionId = regionId;
            CityId = cityId;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            MaxParticipants = maxParticipants;
            CreatorProfileId = creatorProfileId;
            Status = EventStatus.Scheduled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Название мероприятия.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// URL изображения.
        /// </summary>
        public string? ImageUrl { get; private set; }

        /// <summary>
        /// Идентификатор региона.
        /// </summary>
        public int RegionId { get; private set; }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public int CityId { get; private set; }

        /// <summary>
        /// Адрес проведения.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Дата и время начала.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// Дата и время окончания (может быть не указано).
        /// </summary>
        public DateTime? EndDateTime { get; private set; }

        /// <summary>
        /// Максимальное количество участников (0 — без ограничений).
        /// </summary>
        public int MaxParticipants { get; private set; }

        /// <summary>
        /// Текущий статус мероприятия.
        /// </summary>
        public EventStatus Status { get; private set; }

        /// <summary>
        /// Идентификатор профиля создателя.
        /// </summary>
        public Guid CreatorProfileId { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Дата последнего обновления.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Список регистраций на мероприятие.
        /// </summary>
        public IReadOnlyCollection<EventRegistration> Registrations => _registrations.AsReadOnly();

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        // Навигационные свойства (будут загружаться отдельно)
        public Region? Region { get; private set; }
        public City? City { get; private set; }
        public MusicianProfile? CreatorProfile { get; private set; }

        // ---------- Бизнес-методы ----------

        public void Register(Guid profileId)
        {
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Нельзя зарегистрироваться на отменённое или завершённое мероприятие.");

            if (_registrations.Any(r => r.ProfileId == profileId))
                throw new DomainException("Пользователь уже зарегистрирован на это мероприятие.");

            if (MaxParticipants > 0 && _registrations.Count >= MaxParticipants)
                throw new DomainException("Достигнут лимит участников.");

            if (StartDateTime < DateTime.UtcNow)
                throw new DomainException("Мероприятие уже началось, регистрация невозможна.");

            _registrations.Add(new EventRegistration(Id, profileId));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unregister(Guid profileId)
        {
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Нельзя отменить регистрацию на отменённое или завершённое мероприятие.");

            var registration = _registrations.FirstOrDefault(r => r.ProfileId == profileId);
            if (registration == null)
                throw new DomainException("Пользователь не зарегистрирован на это мероприятие.");

            _registrations.Remove(registration);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(Guid userId)
        {
            if (CreatorProfileId != userId)
                throw new DomainException("Только создатель может отменить мероприятие.");

            if (Status != EventStatus.Scheduled)
                throw new DomainException("Мероприятие уже отменено или завершено.");

            Status = EventStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(
            string title,
            string? description,
            int regionId,
            int cityId,
            string address,
            DateTime startDateTime,
            DateTime? endDateTime,
            int maxParticipants,
            Guid userId)
        {
            if (CreatorProfileId != userId)
                throw new DomainException("Только создатель может редактировать мероприятие.");

            if (Status != EventStatus.Scheduled)
                throw new DomainException("Редактировать можно только запланированное мероприятие.");

            if (endDateTime.HasValue && endDateTime.Value < startDateTime)
                throw new DomainException("Дата окончания не может быть раньше даты начала.");

            Title = title;
            Description = description;
            RegionId = regionId;
            CityId = cityId;
            Address = address;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            MaxParticipants = maxParticipants;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetImage(string imageUrl, Guid userId)
        {
            if (CreatorProfileId != userId)
                throw new DomainException("Только создатель может изменять изображение.");

            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}
