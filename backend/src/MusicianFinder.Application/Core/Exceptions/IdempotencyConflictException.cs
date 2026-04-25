namespace MusicianFinder.Application.Core.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о том, что запрос с переданным ключом идемпотентности уже находится в обработке (409 Conflict).
    /// </summary>
    public class IdempotencyConflictException : ConflictException
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IdempotencyConflictException"/>.
        /// </summary>
        public IdempotencyConflictException()
            : base("Запрос с этим ключом идемпотентности уже выполняется.") { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IdempotencyConflictException"/> с указанным сообщением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public IdempotencyConflictException(string message) : base(message) { }
    }
}