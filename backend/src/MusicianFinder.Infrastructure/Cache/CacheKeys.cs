namespace MusicianFinder.Infrastructure.Cache
{
    /// <summary>
    /// Константы ключей кеша, используемые в приложении.
    /// </summary>
    public static class CacheKeys
    {
        /// <summary>Ключ для профиля по идентификатору.</summary>
        public static string Profile(Guid id) => $"profile:{id}";

        /// <summary>Ключ для мероприятия по идентификатору.</summary>
        public static string Event(Guid id) => $"event:{id}";

        /// <summary>Ключ для списка всех мероприятий.</summary>
        public const string EventsList = "events:list";

        /// <summary>Ключ для справочника городов.</summary>
        public const string Cities = "reference:cities";

        /// <summary>Ключ для справочника регионов.</summary>
        public const string Regions = "reference:regions";

        /// <summary>Ключ для справочника жанров.</summary>
        public const string Genres = "reference:genres";

        /// <summary>Ключ для справочника специальностей.</summary>
        public const string Specialties = "reference:specialties";

        /// <summary>Ключ для справочника целей сотрудничества.</summary>
        public const string CollaborationGoals = "reference:collaborationgoals";

        /// <summary>Ключ для избранного пользователя.</summary>
        public static string Favorites(Guid profileId) => $"favorites:{profileId}";

        /// <summary>Ключ для уведомлений профиля.</summary>
        public static string Notifications(Guid profileId) => $"notifications:{profileId}";
    }
}