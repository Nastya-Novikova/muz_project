using System;

namespace MusicianFinder.Application.Common.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о недостатке прав для выполнения операции.
    /// </summary>
    public class ForbiddenException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ForbiddenException"/>.
        /// </summary>
        public ForbiddenException() : base("Доступ запрещён.") { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ForbiddenException"/> с указанным сообщением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public ForbiddenException(string message) : base(message) { }
    }
}