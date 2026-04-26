namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для работы с распределённым кешем.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Получает данные из кеша.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="key">Ключ кеша.</param>
        /// <returns>Данные или null.</returns>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Сохраняет значение в кеш.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="key">Ключ кеша.</param>
        /// <param name="value">Значение.</param>
        /// <param name="expiration">Время жизни записи.</param>
        Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;

        /// <summary>
        /// Удаляет запись из кеша.
        /// </summary>
        /// <param name="key">Ключ кеша.</param>
        Task RemoveAsync(string key);
    }
}