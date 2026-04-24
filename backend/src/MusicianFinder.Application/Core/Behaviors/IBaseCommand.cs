namespace MusicianFinder.Application.Core.Behaviors
{
    /// <summary>
    /// Базовый интерфейс для команд, поддерживающих транзакционность и идемпотентность.
    /// </summary>
    public interface IBaseCommand
    {
        /// <summary>
        /// Уникальный ключ идемпотентности, гарантирующий однократное выполнение команды.
        /// </summary>
        string IdempotencyKey { get; set; }
    }
}