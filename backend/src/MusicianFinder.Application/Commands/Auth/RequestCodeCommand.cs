using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Команда для запроса кода подтверждения на email.
    /// </summary>
    public class RequestCodeCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Email, на который будет выслан код подтверждения.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}