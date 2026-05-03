using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для записи музыкальных профилей.
    /// </summary>
    public interface IMusicianProfileRepository
    {
        /// <summary>
        /// Получает профиль по идентификатору пользователя-владельца.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Профиль музыканта или null, если не найден.</returns>
        Task<MusicianProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<MusicianProfile?> GetByIdAsync(Guid profileId, CancellationToken ct = default);

        /// <summary>
        /// Добавляет новый профиль.
        /// </summary>
        /// <param name="profile">Экземпляр профиля.</param>
        void Add(MusicianProfile profile);


        Task AddNotificationAsync(Guid profileId, Notification notification, CancellationToken ct = default);

        /// <summary>
        /// Выполняет доменную операцию, создающую новую owned-сущность,
        /// и гарантирует, что она будет сохранена как новая запись.
        /// </summary>
        /// <typeparam name="T">Тип owned-сущности.</typeparam>
        /// <param name="userId">Идентификатор пользователя-владельца профиля.</param>
        /// <param name="domainOperation">
        /// Делегат, принимающий профиль и возвращающий созданную owned-сущность.
        /// </param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Задача, завершающаяся после выполнения операции.</returns>
        Task ExecuteAndTrackNewOwnedAsync<T>(
            Guid userId,
            Func<MusicianProfile, T> domainOperation,
            CancellationToken ct = default)
            where T : class;
    }
}