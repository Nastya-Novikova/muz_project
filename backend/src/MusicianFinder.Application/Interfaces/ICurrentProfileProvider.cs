using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Предоставляет профиль текущего аутентифицированного пользователя.
    /// Гарантирует, что пользователь авторизован и профиль существует, иначе выбрасывает исключение.
    /// </summary>
    public interface ICurrentProfileProvider
    {
        /// <summary>
        /// Возвращает профиль музыканта для текущего пользователя.
        /// </summary>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Профиль музыканта.</returns>
        /// <exception cref="ForbiddenException">Если пользователь не аутентифицирован.</exception>
        /// <exception cref="NotFoundException">Если профиль не найден.</exception>
        Task<MusicianProfile> GetCurrentProfileAsync(CancellationToken ct = default);
    }
}