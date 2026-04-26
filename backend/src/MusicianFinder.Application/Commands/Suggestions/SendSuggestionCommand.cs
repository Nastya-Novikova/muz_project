using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Команда для отправки предложения о сотрудничестве.
    /// </summary>
    public class SendSuggestionCommand : ICommand<Guid>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор получателя.
        /// </summary>
        public Guid ToProfileId { get; set; }

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string? Message { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}