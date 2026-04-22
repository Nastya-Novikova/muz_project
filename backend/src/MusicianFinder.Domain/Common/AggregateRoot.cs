using System.Collections.Generic;

namespace MusicianFinder.Domain.Common
{
    /// <summary>
    /// Базовый класс для корней агрегатов. Содержит коллекцию доменных событий.
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        /// <summary>
        /// Коллекция доменных событий, произошедших с агрегатом.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Добавляет доменное событие в коллекцию.
        /// </summary>
        /// <param name="domainEvent">Доменное событие.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Очищает коллекцию доменных событий.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}