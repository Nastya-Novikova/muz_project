using MusicianFinder.Application.DTOs.Auth;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения данных пользователей.
    /// </summary>
    public interface IUserReadRepository
    {
        /// <summary>
        /// Получает DTO пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>DTO пользователя или null.</returns>
        Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}