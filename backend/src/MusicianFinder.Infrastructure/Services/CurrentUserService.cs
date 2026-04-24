using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис получения информации о текущем пользователе из HTTP-контекста.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        /// <inheritdoc />
        public Guid UserId { get; } = Guid.Empty;

        /// <inheritdoc />
        public string Email { get; } = string.Empty;

        /// <inheritdoc />
        public string Role { get; } = string.Empty;

        /// <inheritdoc />
        public bool IsAuthenticated => false;
    }
}