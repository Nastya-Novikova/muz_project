namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для получения информации о текущем аутентифицированном пользователе.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Идентификатор текущего пользователя.
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// Email текущего пользователя.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Роль текущего пользователя.
        /// </summary>
        string Role { get; }

        /// <summary>
        /// Признак аутентификации пользователя.
        /// </summary>
        bool IsAuthenticated { get; }
    }
}