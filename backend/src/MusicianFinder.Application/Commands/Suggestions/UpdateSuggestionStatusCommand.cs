using MediatR;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Команда для изменения статуса предложения о сотрудничестве.
    /// </summary>
    public class UpdateSuggestionStatusCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор предложения.
        /// </summary>
        public Guid SuggestionId { get; set; }

        /// <summary>
        /// Новый статус (Accepted или Rejected).
        /// </summary>
        public SuggestionStatus Status { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}