using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие принятия предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    public sealed record CollaborationSuggestionAccepted(Guid SuggestionId) : IDomainEvent;
}