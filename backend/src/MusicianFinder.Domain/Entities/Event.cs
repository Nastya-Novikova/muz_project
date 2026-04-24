using System;
using System.Collections.Generic;
using System.Linq;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Мероприятие. Корень агрегата.
    /// </summary>
    public class Event : AggregateRoot, ISoftDeletable
    {
        private readonly List<EventRegistration> _registrations = [];

        private Event()
        {
            Title = string.Empty;
            Address = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр мероприятия.
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="address">Адрес.</param>
        /// <param name="startDateTime">Дата и время начала.</param>
        /// <param name="creatorProfileId">Идентификатор профиля создателя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="endDateTime">Дата и время окончания.</param>
        /// <param name="maxParticipants">Максимальное количество участников (0 — без ограничений).</param>
        /// <exception cref="DomainException">Выбрасывается при нарушении бизнес-правил.</exception>
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
            SetTitle(title);
            SetAddress(address);
            if (startDateTime <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");

            Id = Guid.NewGuid();
            Description = description;
            RegionId = regionId;
            CityId = cityId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            MaxParticipants = maxParticipants;
            CreatorProfileId = creatorProfileId;
            Status = EventStatus.Scheduled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;

            AddDomainEvent(new EventCreatedDomainEvent(Id));
        }

        /// <summary>
        /// Уникальный идентификатор мероприятия.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Название мероприятия.
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// Описание мероприятия.
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
        public string Address { get; private set; } = string.Empty;

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
        /// Статус мероприятия.
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
        /// Список регистраций на мероприятие (только для чтения).
        /// </summary>
        public IReadOnlyCollection<EventRegistration> Registrations => _registrations.AsReadOnly();

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Регион (навигационное свойство).
        /// </summary>
        public Region? Region { get; private set; }

        /// <summary>
        /// Город (навигационное свойство).
        /// </summary>
        public City? City { get; private set; }

        /// <summary>
        /// Профиль создателя (навигационное свойство).
        /// </summary>
        public MusicianProfile? CreatorProfile { get; private set; }

        /// <summary>
        /// Регистрирует пользователя на мероприятие.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <exception cref="DomainException">Выбрасывается при невозможности регистрации.</exception>
        public void Register(Guid profileId)
        {
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Нельзя зарегистрироваться на отменённое или завершённое мероприятие.");

            if (_registrations.Any(r => r.ProfileId == profileId))
                throw new DomainException("Пользователь уже зарегистрирован.");

            if (MaxParticipants > 0 && _registrations.Count >= MaxParticipants)
                throw new DomainException("Достигнут лимит участников.");

            if (StartDateTime < DateTime.UtcNow)
                throw new DomainException("Мероприятие уже началось.");

            _registrations.Add(new EventRegistration(Id, profileId));
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserRegisteredToEventDomainEvent(Id, profileId));
        }

        /// <summary>
        /// Отменяет регистрацию пользователя.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <exception cref="DomainException">Выбрасывается, если пользователь не зарегистрирован.</exception>
        public void Unregister(Guid profileId)
        {
            if (Status != EventStatus.Scheduled)
                throw new DomainException("Нельзя отменить регистрацию на отменённое или завершённое мероприятие.");

            var registration = _registrations.FirstOrDefault(r => r.ProfileId == profileId)
                ?? throw new DomainException("Пользователь не зарегистрирован.");

            _registrations.Remove(registration);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отменяет мероприятие. Может выполнить только создатель.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, выполняющего отмену.</param>
        /// <exception cref="DomainException">Выбрасывается, если пользователь не создатель или мероприятие уже отменено/завершено.</exception>
        public void Cancel(Guid userId)
        {
            if (CreatorProfileId != userId)
                throw new DomainException("Только создатель может отменить мероприятие.");

            if (Status != EventStatus.Scheduled)
                throw new DomainException("Мероприятие уже отменено или завершено.");

            Status = EventStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new EventCancelledDomainEvent(Id));
        }

        /// <summary>
        /// Обновляет информацию о мероприятии. Может выполнить только создатель.
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="description">Описание.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="address">Адрес.</param>
        /// <param name="startDateTime">Дата начала.</param>
        /// <param name="endDateTime">Дата окончания.</param>
        /// <param name="maxParticipants">Максимум участников.</param>
        /// <param name="userId">Идентификатор пользователя, выполняющего обновление.</param>
        /// <exception cref="DomainException">Выбрасывается при нарушении бизнес-правил.</exception>
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

            SetTitle(title);
            SetAddress(address);
            if (startDateTime <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");
            if (endDateTime.HasValue && endDateTime.Value < startDateTime)
                throw new DomainException("Дата окончания не может быть раньше даты начала.");

            Description = description;
            RegionId = regionId;
            CityId = cityId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            MaxParticipants = maxParticipants;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new EventUpdatedDomainEvent(Id));
        }

        /// <summary>
        /// Устанавливает изображение мероприятия. Может выполнить только создатель.
        /// </summary>
        /// <param name="imageUrl">URL изображения.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <exception cref="DomainException">Выбрасывается, если пользователь не создатель.</exception>
        public void SetImage(string imageUrl, Guid userId)
        {
            if (CreatorProfileId != userId)
                throw new DomainException("Только создатель может изменять изображение.");

            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new EventUpdatedDomainEvent(Id));
        }

        /// <summary>
        /// Помечает мероприятие как удалённое.
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Название не может быть пустым.");
            if (title.Length > 200)
                throw new DomainException("Название не может быть длиннее 200 символов.");
            Title = title;
        }

        private void SetAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Адрес не может быть пустым.");
            if (address.Length > 200)
                throw new DomainException("Адрес не может быть длиннее 200 символов.");
            Address = address;
        }
    }
}