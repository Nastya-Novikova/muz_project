namespace MusicianFinder.SharedKernel.Constants
{
    /// <summary>
    /// Общие константы приложения.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Длительность действия кода подтверждения email в минутах.
        /// </summary>
        public const int EmailCodeExpirationMinutes = 10;

        /// <summary>
        /// Максимальный размер загружаемого изображения (в байтах) — 5 МБ.
        /// </summary>
        public const long MaxImageSizeBytes = 5 * 1024 * 1024;

        /// <summary>
        /// Максимальный размер аудиофайла (в байтах) — 100 МБ.
        /// </summary>
        public const long MaxAudioSizeBytes = 100 * 1024 * 1024;

        /// <summary>
        /// Максимальный размер видеофайла (в байтах) — 500 МБ.
        /// </summary>
        public const long MaxVideoSizeBytes = 500 * 1024 * 1024;

        /// <summary>
        /// Количество дней, за которые отображаются уведомления.
        /// </summary>
        public const int NotificationRetentionDays = 30;

        /// <summary>
        /// Размер страницы по умолчанию для пагинации.
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// Максимальный размер страницы.
        /// </summary>
        public const int MaxPageSize = 100;
    }
}