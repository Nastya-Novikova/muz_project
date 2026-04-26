namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Базовое исключение предметной области.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainException"/>.
        /// </summary>
        /// <param name="message">Описание ошибки.</param>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainException"/> с внутренним исключением.
        /// </summary>
        /// <param name="message">Описание ошибки.</param>
        /// <param name="innerException">Внутреннее исключение, ставшее причиной этой ошибки.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}