namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Реестр для разрешения CLR-типа интеграционного события по его строковому имени и версии.
    /// </summary>
    public interface IIntegrationEventTypeRegistry
    {
        /// <summary>
        /// Возвращает CLR-тип, соответствующий заданному имени и версии события.
        /// </summary>
        /// <param name="eventName">Строковое имя события, например "profile.created".</param>
        /// <param name="version">Версия события.</param>
        /// <returns>Тип интеграционного события.</returns>
        Type Resolve(string eventName, int version);
    }
}