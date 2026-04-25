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

        /// <summary>
        /// Добавляет новый профиль.
        /// </summary>
        /// <param name="profile">Экземпляр профиля.</param>
        void Add(MusicianProfile profile);

        Task AddPortfolioItemAsync(Guid userId, PortfolioItem item, CancellationToken ct = default);

        Task AddFavoriteAsync(Guid userId, Guid targetProfileId, CancellationToken ct = default);
    }
}