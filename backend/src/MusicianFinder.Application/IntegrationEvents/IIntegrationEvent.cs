namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Контракт интеграционного события, передаваемого во внешнюю шину.
    /// </summary>
    public interface IIntegrationEvent
    {
        /// <summary>
        /// Уникальное имя типа события, например "profile.created".
        /// </summary>
        string EventName { get; }
        /// <summary>
        /// Версия формата события.
        /// </summary>
        int Version { get; }
    }
}