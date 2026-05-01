using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Команда для входа/регистрации по коду подтверждения.
    /// </summary>
    public class LoginCommand : ICommand<AuthResponse>, IBaseCommand
    {
        /// <summary>
        /// Email пользователя для входа.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Шестизначный код подтверждения, отправленный на email.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}