using MediatR;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Команда для отправки предложения о сотрудничестве.
    /// </summary>
    public class SendSuggestionCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор профиля получателя.
        /// </summary>
        public Guid ToProfileId { get; set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; set; }
    }
}