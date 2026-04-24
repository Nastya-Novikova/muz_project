namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Базовый класс для корней агрегатов.
    /// Управляет коллекцией доменных событий.
    /// </summary>
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Неизменяемая коллекция доменных событий, произошедших с агрегатом.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Добавляет доменное событие в агрегат.
        /// </summary>
        /// <param name="domainEvent">Экземпляр доменного события.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Очищает все накопленные доменные события.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}