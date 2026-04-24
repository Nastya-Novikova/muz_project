using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Название мероприятия.
    /// </summary>
    public sealed class EventTitle : ValueObject
    {
        /// <summary>
        /// Строковое значение заголовка.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventTitle"/>.
        /// </summary>
        /// <param name="value">Название. Не должно быть пустым или превышать 200 символов.</param>
        /// <exception cref="DomainException">Выбрасывается при нарушении ограничений.</exception>
        public EventTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Название мероприятия не может быть пустым.");
            if (value.Length > 200)
                throw new DomainException("Название мероприятия не может быть длиннее 200 символов.");
            Value = value;
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() { yield return Value.ToLowerInvariant(); }
    }
}