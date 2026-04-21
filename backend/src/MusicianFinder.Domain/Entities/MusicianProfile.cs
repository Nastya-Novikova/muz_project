using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Профиль музыканта или группы.
    /// </summary>
    public class MusicianProfile : ISoftDeletable
    {
        private readonly List<Genre> _genres = new();
        private readonly List<MusicalSpecialty> _specialties = new();
        private readonly List<CollaborationGoal> _collaborationGoals = new();
        private readonly List<Genre> _desiredGenres = new();
        private readonly List<MusicalSpecialty> _desiredSpecialties = new();
        private readonly List<PortfolioAudio> _audioFiles = new();
        private readonly List<PortfolioVideo> _videoFiles = new();
        private readonly List<PortfolioPhoto> _photos = new();

        private MusicianProfile() { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MusicianProfile"/>.
        /// </summary>
        /// <param name="profileType">Тип профиля.</param>
        /// <param name="fullName">Полное имя / название.</param>
        /// <param name="cityId">Идентификатор города.</param>
        /// <param name="email">Email.</param>
        /// <param name="experience">Опыт в годах.</param>
        /// <param name="lookingFor">Кого ищет.</param>
        public MusicianProfile(
            ProfileType profileType,
            string fullName,
            int cityId,
            string email,
            int experience = 0,
            LookingFor lookingFor = LookingFor.NotLooking)
        {
            Id = Guid.NewGuid();
            ProfileType = profileType;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            CityId = cityId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Experience = experience;
            LookingFor = lookingFor;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            NotifyByEmail = true;
            NotifyByVk = false;
            IsDeleted = false;
        }

        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Тип профиля: индивидуальный музыкант или группа.
        /// </summary>
        public ProfileType ProfileType { get; private set; }

        /// <summary>
        /// Полное имя (или название группы).
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// URL аватара.
        /// </summary>
        public string? AvatarUrl { get; private set; }

        /// <summary>
        /// Возраст (может быть не указан).
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Описание профиля.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Контактный телефон.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Имя пользователя в Telegram.
        /// </summary>
        public string? Telegram { get; set; }

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
        /// Дата последнего обновления профиля.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Город проживания.
        /// </summary>
        public City? City { get; private set; }

        /// <summary>
        /// Жанры, которые предлагает музыкант.
        /// </summary>
        public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();

        /// <summary>
        /// Специальности, которыми владеет музыкант.
        /// </summary>
        public IReadOnlyCollection<MusicalSpecialty> Specialties => _specialties.AsReadOnly();

        /// <summary>
        /// Цели сотрудничества.
        /// </summary>
        public IReadOnlyCollection<CollaborationGoal> CollaborationGoals => _collaborationGoals.AsReadOnly();

        /// <summary>
        /// Жанры, которые ищет музыкант.
        /// </summary>
        public IReadOnlyCollection<Genre> DesiredGenres => _desiredGenres.AsReadOnly();

        /// <summary>
        /// Специальности, которые ищет музыкант.
        /// </summary>
        public IReadOnlyCollection<MusicalSpecialty> DesiredSpecialties => _desiredSpecialties.AsReadOnly();

        /// <summary>
        /// Аудиозаписи в портфолио.
        /// </summary>
        public IReadOnlyCollection<PortfolioAudio> AudioFiles => _audioFiles.AsReadOnly();

        /// <summary>
        /// Видеозаписи в портфолио.
        /// </summary>
        public IReadOnlyCollection<PortfolioVideo> VideoFiles => _videoFiles.AsReadOnly();

        /// <summary>
        /// Фотографии в портфолио.
        /// </summary>
        public IReadOnlyCollection<PortfolioPhoto> Photos => _photos.AsReadOnly();

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
            if (!string.IsNullOrWhiteSpace(fullName)) FullName = fullName;
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
        public void AddGenre(Genre genre)
        {
            if (genre == null) throw new ArgumentNullException(nameof(genre));
            if (!_genres.Contains(genre))
                _genres.Add(genre);
        }

        /// <summary>
        /// Очищает список предлагаемых жанров.
        /// </summary>
        public void ClearGenres() => _genres.Clear();

        /// <summary>
        /// Добавляет специальность в список предлагаемых специальностей.
        /// </summary>
        /// <param name="specialty">Специальность.</param>
        public void AddSpecialty(MusicalSpecialty specialty)
        {
            if (specialty == null) throw new ArgumentNullException(nameof(specialty));
            if (!_specialties.Contains(specialty))
                _specialties.Add(specialty);
        }

        /// <summary>
        /// Очищает список предлагаемых специальностей.
        /// </summary>
        public void ClearSpecialties() => _specialties.Clear();

        /// <summary>
        /// Добавляет цель сотрудничества.
        /// </summary>
        /// <param name="goal">Цель сотрудничества.</param>
        public void AddCollaborationGoal(CollaborationGoal goal)
        {
            if (goal == null) throw new ArgumentNullException(nameof(goal));
            if (!_collaborationGoals.Contains(goal))
                _collaborationGoals.Add(goal);
        }

        /// <summary>
        /// Очищает список целей сотрудничества.
        /// </summary>
        public void ClearCollaborationGoals() => _collaborationGoals.Clear();

        /// <summary>
        /// Добавляет жанр в список искомых жанров.
        /// </summary>
        /// <param name="genre">Жанр.</param>
        public void AddDesiredGenre(Genre genre)
        {
            if (genre == null) throw new ArgumentNullException(nameof(genre));
            if (!_desiredGenres.Contains(genre))
                _desiredGenres.Add(genre);
        }

        /// <summary>
        /// Очищает список искомых жанров.
        /// </summary>
        public void ClearDesiredGenres() => _desiredGenres.Clear();

        /// <summary>
        /// Добавляет специальность в список искомых специальностей.
        /// </summary>
        /// <param name="specialty">Специальность.</param>
        public void AddDesiredSpecialty(MusicalSpecialty specialty)
        {
            if (specialty == null) throw new ArgumentNullException(nameof(specialty));
            if (!_desiredSpecialties.Contains(specialty))
                _desiredSpecialties.Add(specialty);
        }

        /// <summary>
        /// Очищает список искомых специальностей.
        /// </summary>
        public void ClearDesiredSpecialties() => _desiredSpecialties.Clear();

        /// <summary>
        /// Добавляет аудиозапись в портфолио.
        /// </summary>
        /// <param name="audio">Аудиозапись.</param>
        public void AddAudio(PortfolioAudio audio)
        {
            if (audio == null) throw new ArgumentNullException(nameof(audio));
            _audioFiles.Add(audio);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Добавляет видеозапись в портфолио.
        /// </summary>
        /// <param name="video">Видеозапись.</param>
        public void AddVideo(PortfolioVideo video)
        {
            if (video == null) throw new ArgumentNullException(nameof(video));
            _videoFiles.Add(video);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Добавляет фотографию в портфолио.
        /// </summary>
        /// <param name="photo">Фотография.</param>
        public void AddPhoto(PortfolioPhoto photo)
        {
            if (photo == null) throw new ArgumentNullException(nameof(photo));
            _photos.Add(photo);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Помечает профиль как удалённый.
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        /// <inheritdoc />
        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}