namespace MusicianFinder.Application.Commands.Base
{
    /// <summary>
    /// Базовый интерфейс для команд, поддерживающих идемпотентность.
    /// </summary>
    public interface IBaseCommand
    {
        /// <summary>
        /// Уникальный ключ идемпотентности, гарантирующий однократное выполнение команды.
        /// </summary>
        string IdempotencyKey { get; set; }
    }
}