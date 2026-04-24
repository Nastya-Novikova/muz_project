using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Команда для запроса кода подтверждения на email.
    /// </summary>
    public class RequestCodeCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Email для отправки кода.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}