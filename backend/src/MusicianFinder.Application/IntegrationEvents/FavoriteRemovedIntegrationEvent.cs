namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие удаления профиля из избранного.
    /// </summary>
    /// <param name="AddedByProfileId">Идентификатор профиля, удалившего из избранного.</param>
    /// <param name="TargetProfileId">Идентификатор профиля, удалённого из избранного.</param>
    public sealed record FavoriteRemovedIntegrationEvent(Guid AddedByProfileId, Guid TargetProfileId) : IIntegrationEvent
    {
        public string EventName => "favorite.removed";
        public int Version => 1;
    }
}