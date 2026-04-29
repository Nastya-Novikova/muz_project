using MusicianFinder.SharedKernel;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Профиль музыканта или коллектива. Корень агрегата.
    /// </summary>
    public class MusicianProfile : AggregateRoot, ISoftDeletable
    {
        private readonly List<GenreId> _genreIds = new();
        private readonly List<SpecialtyId> _specialtyIds = new();
        private readonly List<CollaborationGoalId> _collaborationGoalIds = new();
        private readonly List<GenreId> _desiredGenreIds = new();
        private readonly List<SpecialtyId> _desiredSpecialtyIds = new();
        private readonly List<PortfolioItem> _portfolio = new();
        private readonly List<Favorite> _favorites = new();
        private readonly List<Notification> _notifications = new();

        private MusicianProfile()
        {
            FullName = null!;
            Email = null!;
        }

        private MusicianProfile(Guid userId, ProfileName fullName, int cityId, string email, ProfileType profileType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            FullName = fullName;
            CityId = cityId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            CreatedAt = DateTime.UtcNow;
            ProfileType = profileType;
        }

        public ProfileType ProfileType { get; private set; }

        /// <summary>Идентификатор связанного пользователя.</summary>
        public Guid UserId { get; private set; }

        /// <summary>Полное имя / название.</summary>
        public ProfileName FullName { get; private set; }

        /// <summary>Возраст.</summary>
        public int? Age { get; private set; }

        /// <summary>Идентификатор города.</summary>
        public int CityId { get; private set; }

        /// <summary>Контактный телефон.</summary>
        public PhoneNumber? Phone { get; private set; }

        /// <summary>Имя пользователя в Telegram.</summary>
        public TelegramHandle? Telegram { get; private set; }

        /// <summary>Идентификатор ВКонтакте.</summary>
        public VkUserId? VkUserId { get; private set; }

        /// <summary>Описание профиля.</summary>
        public string? Description { get; private set; }

        /// <summary>URL аватара.</summary>
        public string? AvatarUrl { get; private set; }

        /// <summary>Опыт в годах.</summary>
        public int Experience { get; private set; }

        /// <summary>Кого ищет.</summary>
        public LookingFor LookingFor { get; private set; }

        /// <summary>Email.</summary>
        public string Email { get; private set; }

        /// <summary>Согласие на email-уведомления.</summary>
        public bool NotifyByEmail { get; private set; } = true;

        /// <summary>Согласие на VK-уведомления.</summary>
        public bool NotifyByVk { get; private set; }

        /// <summary>Дата создания.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Дата последнего обновления.</summary>
        public DateTime UpdatedAt { get; private set; }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>Жанры, которые предлагает музыкант.</summary>
        public IReadOnlyCollection<GenreId> GenreIds => _genreIds.AsReadOnly();

        /// <summary>Специальности.</summary>
        public IReadOnlyCollection<SpecialtyId> SpecialtyIds => _specialtyIds.AsReadOnly();

        /// <summary>Цели сотрудничества.</summary>
        public IReadOnlyCollection<CollaborationGoalId> CollaborationGoalIds => _collaborationGoalIds.AsReadOnly();

        /// <summary>Искомые жанры.</summary>
        public IReadOnlyCollection<GenreId> DesiredGenreIds => _desiredGenreIds.AsReadOnly();

        /// <summary>Искомые специальности.</summary>
        public IReadOnlyCollection<SpecialtyId> DesiredSpecialtyIds => _desiredSpecialtyIds.AsReadOnly();

        /// <summary>Элементы портфолио.</summary>
        public IReadOnlyCollection<PortfolioItem> Portfolio => _portfolio.AsReadOnly();

        /// <summary>Избранное, добавленное этим профилем.</summary>
        public IReadOnlyCollection<Favorite> Favorites => _favorites.AsReadOnly();

        /// <summary>Уведомления, адресованные профилю.</summary>
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        /// <summary>
        /// Создаёт новый профиль и добавляет событие <see cref="ProfileCreated"/>.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя-владельца.</param>
        /// <param name="fullName">Полное имя / название.</param>
        /// <param name="cityId">Идентификатор города.</param>
        public static MusicianProfile Create(Guid userId, ProfileName fullName, int cityId, string email, ProfileType profileType)
        {
            var profile = new MusicianProfile(userId, fullName, cityId, email, profileType);
            profile.AddDomainEvent(new ProfileCreated(profile.Id, profile.UserId));
            return profile;
        }

        /// <summary>
        /// Обновляет основную информацию профиля.
        /// </summary>
        /// <param name="fullName">Новое полное имя.</param>
        /// <param name="age">Новый возраст.</param>
        /// <param name="description">Новое описание.</param>
        /// <param name="cityId">Новый идентификатор города.</param>
        public void UpdateCoreInfo(ProfileName fullName, int? age, string? description, int cityId)
        {
            FullName = fullName;
            Age = age;
            Description = description;
            CityId = cityId;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileCoreInfoUpdated(Id));
        }

        /// <summary>
        /// Обновляет контактные данные.
        /// </summary>
        /// <param name="phone">Новый телефон.</param>
        /// <param name="telegram">Новый Telegram.</param>
        public void UpdateContacts(PhoneNumber? phone, TelegramHandle? telegram)
        {
            Phone = phone;
            Telegram = telegram;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileContactsUpdated(Id));
        }

        public void SetExperience(int experience) => Experience = experience;

        public void SetLookingFor(LookingFor lookingFor) => LookingFor = lookingFor;

        public void SetProfileType(ProfileType type) => ProfileType = type;

        /// <summary>
        /// Заменяет набор предлагаемых жанров.
        /// </summary>
        /// <param name="genreIds">Новый набор жанров.</param>
        public void SetGenres(IEnumerable<GenreId> genreIds)
        {
            _genreIds.Clear();
            _genreIds.AddRange(genreIds);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileGenresChanged(Id));
        }

        /// <summary>
        /// Заменяет набор специальностей.
        /// </summary>
        /// <param name="ids">Новый набор специальностей.</param>
        public void SetSpecialties(IEnumerable<SpecialtyId> ids)
        {
            _specialtyIds.Clear();
            _specialtyIds.AddRange(ids);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileSpecialtiesChanged(Id));
        }

        /// <summary>
        /// Заменяет набор целей сотрудничества.
        /// </summary>
        /// <param name="ids">Новый набор целей.</param>
        public void SetCollaborationGoals(IEnumerable<CollaborationGoalId> ids)
        {
            _collaborationGoalIds.Clear();
            _collaborationGoalIds.AddRange(ids);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileCollaborationGoalsChanged(Id));
        }

        /// <summary>
        /// Заменяет набор искомых жанров.
        /// </summary>
        /// <param name="ids">Новый набор искомых жанров.</param>
        public void SetDesiredGenres(IEnumerable<GenreId> ids)
        {
            _desiredGenreIds.Clear();
            _desiredGenreIds.AddRange(ids);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileDesiredGenresChanged(Id));
        }

        /// <summary>
        /// Заменяет набор искомых специальностей.
        /// </summary>
        /// <param name="ids">Новый набор искомых специальностей.</param>
        public void SetDesiredSpecialties(IEnumerable<SpecialtyId> ids)
        {
            _desiredSpecialtyIds.Clear();
            _desiredSpecialtyIds.AddRange(ids);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileDesiredSpecialtiesChanged(Id));
        }

        /// <summary>
        /// Добавляет элемент в портфолио.
        /// </summary>
        /// <param name="item">Элемент портфолио.</param>
        public void AddPortfolioItem(PortfolioItem item)
        {
            _portfolio.Add(item);
            //UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new PortfolioItemAdded(Id, item.Id));
        }

        /// <summary>
        /// Удаляет элемент портфолио по идентификатору.
        /// </summary>
        /// <param name="itemId">Идентификатор удаляемого элемента.</param>
        public void RemovePortfolioItem(Guid itemId)
        {
            var removed = _portfolio.RemoveAll(p => p.Id == itemId);
            if (removed > 0)
            {
                UpdatedAt = DateTime.UtcNow;
                AddDomainEvent(new PortfolioItemRemoved(Id, itemId));
            }
        }

        /// <summary>
        /// Устанавливает URL аватара.
        /// </summary>
        /// <param name="avatarUrl">URL аватара.</param>
        public void SetAvatar(string avatarUrl) => AvatarUrl = avatarUrl;

        /// <summary>
        /// Устанавливает идентификатор ВКонтакте.
        /// </summary>
        /// <param name="vkUserId">Идентификатор VK.</param>
        public void SetVkUserId(VkUserId vkUserId)
        {

            VkUserId = vkUserId;
            NotifyByVk = true;
        }

        /// <summary>
        /// Добавляет профиль в избранное.
        /// </summary>
        /// <param name="targetProfileId">Идентификатор профиля, добавляемого в избранное.</param>
        public void AddToFavorites(Guid targetProfileId)
        {
            if (_favorites.Any(f => f.TargetProfileId == targetProfileId))
                throw new DomainException("Этот профиль уже в избранном.");
            _favorites.Add(new Favorite(Id, targetProfileId));
            AddDomainEvent(new FavoriteAdded(Id, targetProfileId));
        }

        /// <summary>
        /// Удаляет профиль из избранного.
        /// </summary>
        /// <param name="targetProfileId">Идентификатор удаляемого из избранного профиля.</param>
        public void RemoveFromFavorites(Guid targetProfileId)
        {
            var fav = _favorites.FirstOrDefault(f => f.TargetProfileId == targetProfileId)
                ?? throw new DomainException("Профиль не найден в избранном.");
            _favorites.Remove(fav);
            AddDomainEvent(new FavoriteRemoved(Id, targetProfileId));
        }

        /// <summary>
        /// Добавляет уведомление в коллекцию профиля.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        public void AddNotification(Notification notification)
        {
            _notifications.Add(notification);
        }

        public void UpdateNotificationPreferences(bool notifyByEmail, bool notifyByVk)
        {
            NotifyByEmail = notifyByEmail;
            NotifyByVk = notifyByVk;
        }

        /// <summary>
        /// Помечает профиль как удалённый (мягкое удаление).
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileDeleted(Id));
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}