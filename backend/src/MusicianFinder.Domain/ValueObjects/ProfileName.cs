using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Имя профиля музыканта или коллектива.
    /// </summary>
    public sealed class ProfileName : ValueObject
    {
        /// <summary>
        /// Строковое значение имени.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfileName"/>.
        /// </summary>
        /// <param name="value">Имя профиля. Не должно быть пустым или превышать 100 символов.</param>
        /// <exception cref="DomainException">Выбрасывается при нарушении ограничений.</exception>
        public ProfileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Имя профиля не может быть пустым.");
            if (value.Length > 100)
                throw new DomainException("Имя профиля не может быть длиннее 100 символов.");
            Value = value;
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }
    }
}