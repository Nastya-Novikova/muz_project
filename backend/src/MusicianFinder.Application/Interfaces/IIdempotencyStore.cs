namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Запись хранилища идемпотентности.
    /// </summary>
    public class IdempotencyRecord
    {
        /// <summary>Ключ идемпотентности.</summary>
        public string Key { get; set; } = default!;
        /// <summary>Хеш запроса.</summary>
        public string RequestHash { get; set; } = default!;
        /// <summary>Сериализованный ответ (заполняется после выполнения).</summary>
        public string? Response { get; set; }
        /// <summary>Статус: InProgress, Completed.</summary>
        public string Status { get; set; } = "InProgress";
    }

    /// <summary>
    /// Хранилище записей идемпотентности команд.
    /// </summary>
    public interface IIdempotencyStore
    {
        /// <summary>
        /// Пытается атомарно создать новую запись идемпотентности.
        /// Возвращает признак успеха создания и существующую запись (если уже была).
        /// </summary>
        /// <param name="key">Ключ идемпотентности.</param>
        /// <param name="requestHash">Хеш тела запроса.</param>
        /// <returns>Кортеж (создана ли новая запись, существующая запись или null).</returns>
        Task<(bool Created, IdempotencyRecord? Record)> TryCreateAsync(string key, string requestHash);

        /// <summary>
        /// Получает запись идемпотентности по ключу.
        /// </summary>
        /// <param name="key">Ключ идемпотентности.</param>
        /// <returns>Запись или null.</returns>
        Task<IdempotencyRecord?> GetAsync(string key);

        /// <summary>
        /// Обновляет запись идемпотентности (обычно сохраняет ответ и статус).
        /// </summary>
        /// <param name="key">Ключ идемпотентности.</param>
        /// <param name="response">Сериализованный ответ.</param>
        /// <param name="status">Новый статус (например, "Completed").</param>
        Task UpdateAsync(string key, string response, string status);
    }
}