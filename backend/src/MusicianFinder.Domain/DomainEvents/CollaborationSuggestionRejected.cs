using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие отклонения предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    public sealed record CollaborationSuggestionRejected(Guid SuggestionId) : IDomainEvent;
}