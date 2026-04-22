using MediatR;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Команда для запроса кода подтверждения на email.
    /// </summary>
    public class RequestCodeCommand : IRequest<Unit>
    {
        /// <summary>
        /// Email для отправки кода.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}