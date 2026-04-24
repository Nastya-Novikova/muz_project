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
    }
}