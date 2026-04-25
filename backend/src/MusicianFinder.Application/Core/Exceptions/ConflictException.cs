namespace MusicianFinder.Application.Core.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о конфликте данных (например, дублировании уникального поля).
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConflictException"/>.
        /// </summary>
        public ConflictException() : base("Конфликт данных.") { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConflictException"/> с указанным сообщением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public ConflictException(string message) : base(message) { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConflictException"/> с сообщением и внутренним исключением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Внутреннее исключение.</param>
        public ConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}