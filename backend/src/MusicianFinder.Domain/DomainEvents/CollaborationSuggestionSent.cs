using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие отправки предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    /// <param name="FromProfileId">Идентификатор отправителя.</param>
    /// <param name="ToProfileId">Идентификатор получателя.</param>
    public sealed record CollaborationSuggestionSent(Guid SuggestionId, Guid FromProfileId, Guid ToProfileId) : IDomainEvent;
}