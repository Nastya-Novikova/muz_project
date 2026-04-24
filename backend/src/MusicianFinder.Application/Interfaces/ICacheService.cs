namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для работы с кешем (Cache-Aside).
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Получить данные из кеша.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="key">Ключ кеша.</param>
        /// <returns>Данные из кеша или default, если ключ не найден.</returns>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Установить значение в кеш.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="key">Ключ кеша.</param>
        /// <param name="value">Значение.</param>
        /// <param name="expiration">Время жизни записи.</param>
        Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;

        /// <summary>
        /// Удалить значение из кеша.
        /// </summary>
        /// <param name="key">Ключ кеша.</param>
        Task RemoveAsync(string key);
    }
}