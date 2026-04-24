namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие добавления профиля в избранное.
    /// </summary>
    /// <param name="AddedByProfileId">Идентификатор профиля, добавившего в избранное.</param>
    /// <param name="TargetProfileId">Идентификатор профиля, добавленного в избранное.</param>
    public sealed record FavoriteAddedIntegrationEvent(Guid AddedByProfileId, Guid TargetProfileId) : IIntegrationEvent
    {
        public string EventName => "favorite.added";
        public int Version => 1;
    }
}