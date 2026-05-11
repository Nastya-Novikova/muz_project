using MusicianFinder.SharedKernel;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Мероприятие. Корень агрегата.
    /// </summary>
    public class Event : AggregateRoot, ISoftDeletable
    {
        private readonly List<EventRegistration> _registrations = new();

        private Event()
        {
            Title = null!;
            Address = null!;
        }

        /// <summary>
        /// Инициализирует новое мероприятие.
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="address">Адрес.</param>
        /// <param name="startDateTime">Дата и время начала.</param>
        /// <param name="creatorProfileId">Идентификатор создателя.</param>
        /// <param name="description">Описание (необязательно).</param>
        /// <param name="endDateTime">Дата и время окончания (необязательно).</param>
        /// <param name="maxParticipants">Максимальное количество участников (0 — без ограничений).</param>
        public Event(
            EventTitle title,
            int regionId,
            int cityId,
            string address,
            DateTime startDateTime,
            Guid creatorProfileId,
            string? description = null,
            DateTime? endDateTime = null,
            int maxParticipants = 0)
        {
            if (startDateTime <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Адрес не может быть пустым.");

            Id = Guid.NewGuid();
            Title = title;
            RegionId = regionId;
            CityId = cityId;
            Address = address;
            StartDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Unspecified);
            if (endDateTime != null)
            {
                EndDateTime = DateTime.SpecifyKind((DateTime)endDateTime, DateTimeKind.Unspecified);
            }
            MaxParticipants = maxParticipants;
            CreatorProfileId = creatorProfileId;
            Description = description;
            Status = EventStatus.Scheduled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;

            AddDomainEvent(new EventCreated(Id));
        }

        /// <summary>Название.</summary>
        public EventTitle Title { get; private set; }

        /// <summary>Описание.</summary>
        public string? Description { get; private set; }

        /// <summary>URL изображения.</summary>
        public string? ImageUrl { get; private set; }

        /// <summary>Идентификатор региона.</summary>
        public int RegionId { get; private set; }

        /// <summary>Идентификатор города.</summary>
        public int CityId { get; private set; }

        /// <summary>Адрес.</summary>
        public string Address { get; private set; }

        /// <summary>Дата и время начала.</summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>Дата и время окончания.</summary>
        public DateTime? EndDateTime { get; private set; }

        /// <summary>Максимальное количество участников (0 — без ограничений).</summary>
        public int MaxParticipants { get; private set; }

        /// <summary>Статус мероприятия.</summary>
        public EventStatus Status { get; private set; }

        /// <summary>Идентификатор создателя (MusicianProfile).</summary>
        public Guid CreatorProfileId { get; private set; }

        /// <summary>Дата создания.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Дата последнего обновления.</summary>
        public DateTime UpdatedAt { get; private set; }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>Регистрации на мероприятие.</summary>
        public IReadOnlyCollection<EventRegistration> Registrations => _registrations.AsReadOnly();

        /// <summary>
        /// Регистрирует профиль на мероприятие и возвращает созданную запись.
        /// </summary>
        /// <param name="profileId">Идентификатор регистрируемого профиля.</param>
        /// <returns>Созданная регистрация <see cref="EventRegistration"/>.</returns>
        /// <exception cref="DomainException">
        /// Если создатель пытается зарегистрироваться, мероприятие не Scheduled,
        /// пользователь уже зарегистрирован, достигнут лимит участников или мероприятие уже началось.
        /// </exception>
        public EventRegistration Register(Guid profileId)
        {
            if (CreatorProfileId == profileId)
                throw new DomainException("Создатель мероприятия не может зарегистрироваться как участник.");
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Нельзя зарегистрироваться на отменённое или завершённое мероприятие.");
            if (_registrations.Any(r => r.ProfileId == profileId))
                throw new DomainException("Пользователь уже зарегистрирован.");
            if (MaxParticipants > 0 && _registrations.Count >= MaxParticipants)
                throw new DomainException("Достигнут лимит участников.");

            // Приводим StartDateTime к UTC для надёжного сравнения
            DateTime startUtc = StartDateTime.Kind == DateTimeKind.Utc
                ? StartDateTime
                : StartDateTime.ToUniversalTime();

            if (startUtc < DateTime.UtcNow)
                throw new DomainException("Мероприятие уже началось.");

            var registration = new EventRegistration(Id, profileId);
            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserRegisteredToEvent(Id, profileId));
            return registration;
        }

        /// <summary>
        /// Отменяет регистрацию профиля на мероприятие.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля, отменяющего регистрацию.</param>
        public void Unregister(Guid profileId)
        {
            var registration = _registrations.FirstOrDefault(r => r.ProfileId == profileId)
                ?? throw new DomainException("Пользователь не зарегистрирован.");
            _registrations.Remove(registration);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserUnregisteredFromEvent(Id, profileId));
        }

        /// <summary>
        /// Отменяет мероприятие (только создатель).
        /// </summary>
        /// <param name="profileId">Идентификатор профиля, выполняющего отмену.</param>
        public void Cancel(Guid profileId)
        {
            if (CreatorProfileId != profileId)
                throw new DomainException("Только создатель может отменить мероприятие.");
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Мероприятие уже отменено или завершено.");

            Status = EventStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new EventCancelled(Id));
        }

        /// <summary>
        /// Обновляет информацию о мероприятии (только создатель).
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="description">Описание.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="address">Адрес.</param>
        /// <param name="startDateTime">Дата начала.</param>
        /// <param name="endDateTime">Дата окончания.</param>
        /// <param name="maxParticipants">Максимальное количество участников.</param>
        /// <param name="profileId">Идентификатор профиля, выполняющего обновление.</param>
        public void Update(
            EventTitle title,
            string? description,
            int regionId,
            int cityId,
            string address,
            DateTime startDateTime,
            DateTime? endDateTime,
            int maxParticipants,
            Guid profileId)
        {
            if (CreatorProfileId != profileId)
                throw new DomainException("Только создатель может редактировать мероприятие.");
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Редактировать можно только запланированное мероприятие.");
            if (startDateTime <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");

            Title = title;
            Description = description;
            RegionId = regionId;
            CityId = cityId;
            Address = address;
            StartDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Unspecified);
            if (endDateTime != null)
            {
                EndDateTime = DateTime.SpecifyKind((DateTime)endDateTime, DateTimeKind.Unspecified);
            }
            MaxParticipants = maxParticipants;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new EventUpdated(Id));
        }

        /// <summary>
        /// Устанавливает изображение мероприятия (только создатель).
        /// </summary>
        /// <param name="imageUrl">URL изображения.</param>
        /// <param name="profileId">Идентификатор профиля, выполняющего операцию.</param>
        public void SetImage(string imageUrl, Guid profileId)
        {
            if (CreatorProfileId != profileId)
                throw new DomainException("Только создатель может изменять изображение.");
            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new EventUpdated(Id));
        }

        /// <summary>
        /// Помечает мероприятие как удалённое (мягкое удаление).
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}