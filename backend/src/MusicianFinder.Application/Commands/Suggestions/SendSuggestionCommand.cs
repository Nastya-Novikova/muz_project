using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Команда для отправки предложения о сотрудничестве.
    /// </summary>
    public class SendSuggestionCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор профиля получателя.
        /// </summary>
        public Guid ToProfileId { get; set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}