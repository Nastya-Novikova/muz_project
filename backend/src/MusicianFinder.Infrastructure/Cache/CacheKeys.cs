namespace MusicianFinder.Infrastructure.Cache
{
    /// <summary>
    /// Ключи кеша, используемые в приложении.
    /// </summary>
    public static class CacheKeys
    {
        /// <summary>
        /// Ключ для профиля пользователя.
        /// </summary>
        public static string Profile(Guid id) => $"profile:{id}";

        /// <summary>
        /// Ключ для мероприятия.
        /// </summary>
        public static string Event(Guid id) => $"event:{id}";

        /// <summary>
        /// Ключ для списка мероприятий.
        /// </summary>
        public const string EventsList = "events:list";

        /// <summary>
        /// Ключ для справочника городов.
        /// </summary>
        public const string Cities = "reference:cities";

        /// <summary>
        /// Ключ для справочника жанров.
        /// </summary>
        public const string Genres = "reference:genres";

        /// <summary>
        /// Ключ для справочника регионов.
        /// </summary>
        public const string Regions = "reference:regions";

        /// <summary>
        /// Ключ для справочника специальностей.
        /// </summary>
        public const string Specialties = "reference:specialties";

        /// <summary>
        /// Ключ для справочника целей сотрудничества.
        /// </summary>
        public const string CollaborationGoals = "reference:collaborationgoals";
    }
}