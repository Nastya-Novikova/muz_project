using System;
using System.Collections.Generic;
using System.Linq;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Профиль музыканта или группы. Корень агрегата.
    /// </summary>
    public class MusicianProfile : AggregateRoot, ISoftDeletable
    {
        private readonly List<Genre> _genres = [];
        private readonly List<MusicalSpecialty> _specialties = [];
        private readonly List<CollaborationGoal> _collaborationGoals = [];
        private readonly List<Genre> _desiredGenres = [];
        private readonly List<MusicalSpecialty> _desiredSpecialties = [];
        private readonly List<PortfolioItem> _portfolioItems = [];

        private MusicianProfile()
        {
            FullName = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр профиля музыканта.
        /// </summary>
        /// <param name="profileType">Тип профиля (индивидуальный или группа).</param>
        /// <param name="fullName">Полное имя / название.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="email">Email.</param>
        /// <param name="experience">Опыт в годах.</param>
        /// <param name="lookingFor">Кого ищет.</param>
        /// <exception cref="DomainException">Выбрасывается при нарушении бизнес-правил.</exception>
        public MusicianProfile(
            ProfileType profileType,
            string fullName,
            int cityId,
            string email,
            int experience = 0,
            LookingFor lookingFor = LookingFor.NotLooking)
        {
            SetFullName(fullName);
            SetEmail(email);

            Id = Guid.NewGuid();
            ProfileType = profileType;
            CityId = cityId;
            Experience = experience;
            LookingFor = lookingFor;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            NotifyByEmail = true;
            NotifyByVk = false;
            IsDeleted = false;
        }

        /// <summary>
        /// Уникальный идентификатор профиля.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Тип профиля (Individual или Band).
        /// </summary>
        public ProfileType ProfileType { get; private set; }

        /// <summary>
        /// Полное имя или название.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// URL аватара.
        /// </summary>
        public string? AvatarUrl { get; private set; }

        /// <summary>
        /// Возраст (может быть не указан).
        /// </summary>
        public int? Age { get; private set; }

        /// <summary>
        /// Описание профиля.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Контактный телефон.
        /// </summary>
        public string? Phone { get; private set; }

        /// <summary>
        /// Имя пользователя в Telegram.
        /// </summary>
        public string? Telegram { get; private set; }

        /// <summary>
        /// Идентификатор пользователя ВКонтакте.
        /// </summary>
        public string? VkUserId { get; private set; }

        /// <summary>
        /// Согласие на получение уведомлений по email.
        /// </summary>
        public bool NotifyByEmail { get; private set; }

        /// <summary>
        /// Согласие на получение уведомлений через ВКонтакте.
        /// </summary>
        public bool NotifyByVk { get; private set; }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public int CityId { get; private set; }

        /// <summary>
        /// Опыт в годах.
        /// </summary>
        public int Experience { get; private set; }

        /// <summary>
        /// Кого ищет пользователь.
        /// </summary>
        public LookingFor LookingFor { get; private set; }

        /// <summary>
        /// Email, связанный с профилем.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Дата создания профиля.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Дата последнего обновления.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Город проживания (навигационное свойство).
        /// </summary>
        public City? City { get; private set; }

        /// <summary>
        /// Коллекция жанров, которые предлагает музыкант.
        /// </summary>
        public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();

        /// <summary>
        /// Коллекция специальностей.
        /// </summary>
        public IReadOnlyCollection<MusicalSpecialty> Specialties => _specialties.AsReadOnly();

        /// <summary>
        /// Коллекция целей сотрудничества.
        /// </summary>
        public IReadOnlyCollection<CollaborationGoal> CollaborationGoals => _collaborationGoals.AsReadOnly();

        /// <summary>
        /// Коллекция искомых жанров.
        /// </summary>
        public IReadOnlyCollection<Genre> DesiredGenres => _desiredGenres.AsReadOnly();

        /// <summary>
        /// Коллекция искомых специальностей.
        /// </summary>
        public IReadOnlyCollection<MusicalSpecialty> DesiredSpecialties => _desiredSpecialties.AsReadOnly();

        /// <summary>
        /// Коллекция элементов портфолио.
        /// </summary>
        public IReadOnlyCollection<PortfolioItem> PortfolioItems => _portfolioItems.AsReadOnly();

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Обновляет базовую информацию профиля.
        /// </summary>
        /// <param name="profileType">Тип профиля.</param>
        /// <param name="fullName">Полное имя.</param>
        /// <param name="age">Возраст.</param>
        /// <param name="description">Описание.</param>
        /// <param name="phone">Телефон.</param>
        /// <param name="telegram">Telegram.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="experience">Опыт.</param>
        /// <param name="lookingFor">Кого ищет.</param>
        /// <param name="notifyByEmail">Уведомления по email.</param>
        /// <param name="notifyByVk">Уведомления по VK.</param>
        public void UpdateBasicInfo(
            ProfileType? profileType,
            string? fullName,
            int? age,
            string? description,
            string? phone,
            string? telegram,
            int? cityId,
            int? experience,
            LookingFor? lookingFor,
            bool? notifyByEmail,
            bool? notifyByVk)
        {
            if (profileType.HasValue) ProfileType = profileType.Value;
            if (!string.IsNullOrWhiteSpace(fullName)) SetFullName(fullName);
            if (age.HasValue) Age = age.Value;
            if (description != null) Description = description;
            if (phone != null) Phone = phone;
            if (telegram != null) Telegram = telegram;
            if (cityId.HasValue) CityId = cityId.Value;
            if (experience.HasValue) Experience = experience.Value;
            if (lookingFor.HasValue) LookingFor = lookingFor.Value;
            if (notifyByEmail.HasValue) NotifyByEmail = notifyByEmail.Value;
            if (notifyByVk.HasValue) NotifyByVk = notifyByVk.Value;

            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Устанавливает URL аватара.
        /// </summary>
        /// <param name="avatarUrl">URL аватара.</param>
        public void SetAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Устанавливает идентификатор пользователя ВКонтакте.
        /// </summary>
        /// <param name="vkUserId">Идентификатор VK.</param>
        public void SetVkUserId(string vkUserId)
        {
            VkUserId = vkUserId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Добавляет жанр в список предлагаемых жанров.
        /// </summary>
        /// <param name="genre">Жанр.</param>
        /// <exception cref="DomainException">Выбрасывается при превышении лимита (10) или дублировании.</exception>
        public void AddGenre(Genre genre)
        {
            ArgumentNullException.ThrowIfNull(genre);
            if (_genres.Any(g => g.Id == genre.Id)) return;
            if (_genres.Count >= 10) throw new DomainException("Нельзя добавить более 10 жанров.");
            _genres.Add(genre);
        }

        /// <summary>
        /// Очищает список предлагаемых жанров.
        /// </summary>
        public void ClearGenres() => _genres.Clear();

        /// <summary>
        /// Добавляет специальность в список предлагаемых.
        /// </summary>
        /// <param name="specialty">Специальность.</param>
        /// <exception cref="DomainException">Выбрасывается при превышении лимита (5) или дублировании.</exception>
        public void AddSpecialty(MusicalSpecialty specialty)
        {
            ArgumentNullException.ThrowIfNull(specialty);
            if (_specialties.Any(s => s.Id == specialty.Id)) return;
            if (_specialties.Count >= 5) throw new DomainException("Нельзя добавить более 5 специальностей.");
            _specialties.Add(specialty);
        }

        /// <summary>
        /// Очищает список специальностей.
        /// </summary>
        public void ClearSpecialties() => _specialties.Clear();

        /// <summary>
        /// Добавляет цель сотрудничества.
        /// </summary>
        /// <param name="goal">Цель.</param>
        public void AddCollaborationGoal(CollaborationGoal goal)
        {
            ArgumentNullException.ThrowIfNull(goal);
            if (_collaborationGoals.Any(g => g.Id == goal.Id)) return;
            _collaborationGoals.Add(goal);
        }

        /// <summary>
        /// Очищает список целей сотрудничества.
        /// </summary>
        public void ClearCollaborationGoals() => _collaborationGoals.Clear();

        /// <summary>
        /// Добавляет искомый жанр.
        /// </summary>
        /// <param name="genre">Жанр.</param>
        public void AddDesiredGenre(Genre genre)
        {
            ArgumentNullException.ThrowIfNull(genre);
            if (_desiredGenres.Any(g => g.Id == genre.Id)) return;
            _desiredGenres.Add(genre);
        }

        /// <summary>
        /// Очищает список искомых жанров.
        /// </summary>
        public void ClearDesiredGenres() => _desiredGenres.Clear();

        /// <summary>
        /// Добавляет искомую специальность.
        /// </summary>
        /// <param name="specialty">Специальность.</param>
        public void AddDesiredSpecialty(MusicalSpecialty specialty)
        {
            ArgumentNullException.ThrowIfNull(specialty);
            if (_desiredSpecialties.Any(s => s.Id == specialty.Id)) return;
            _desiredSpecialties.Add(specialty);
        }

        /// <summary>
        /// Очищает список искомых специальностей.
        /// </summary>
        public void ClearDesiredSpecialties() => _desiredSpecialties.Clear();

        /// <summary>
        /// Добавляет элемент в портфолио.
        /// </summary>
        /// <param name="item">Элемент портфолио.</param>
        public void AddPortfolioItem(PortfolioItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _portfolioItems.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Удаляет элемент из портфолио по идентификатору.
        /// </summary>
        /// <param name="itemId">Идентификатор элемента.</param>
        public void RemovePortfolioItem(Guid itemId)
        {
            var item = _portfolioItems.FirstOrDefault(p => p.Id == itemId);
            if (item != null)
            {
                _portfolioItems.Remove(item);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Помечает профиль как удалённый.
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();

        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Полное имя не может быть пустым.");
            if (fullName.Length > 100)
                throw new DomainException("Полное имя не может быть длиннее 100 символов.");
            FullName = fullName;
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email не может быть пустым.");
            Email = email;
        }
    }
}