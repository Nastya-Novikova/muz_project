using System;

namespace MusicianFinder.Domain.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о нарушении бизнес-правил домена.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр исключения.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр исключения с внутренним исключением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Внутреннее исключение.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}