using MediatR;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Application.DTOs.Auth;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Команда для входа/регистрации по коду подтверждения.
    /// </summary>
    public class LoginCommand : IRequest<AuthResponse>, IBaseCommand
    {
        /// <summary>
        /// Email пользователя.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код подтверждения.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}