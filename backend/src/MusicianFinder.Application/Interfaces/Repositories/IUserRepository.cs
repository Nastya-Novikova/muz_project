using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для записи пользователей.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Получает пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Пользователь или null.</returns>
        Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Получает пользователя по email.
        /// </summary>
        /// <param name="email">Email адрес.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Пользователь или null.</returns>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Добавляет нового пользователя.
        /// </summary>
        /// <param name="user">Экземпляр пользователя.</param>
        void Add(User user);
    }
}