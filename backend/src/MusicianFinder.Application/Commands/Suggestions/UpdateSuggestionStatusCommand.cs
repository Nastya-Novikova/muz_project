using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Команда для изменения статуса предложения (принять/отклонить).
    /// </summary>
    public class UpdateSuggestionStatusCommand : ICommand<Unit>, IBaseCommand
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