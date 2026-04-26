using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Номер телефона в международном формате (E.164).
    /// </summary>
    public sealed class PhoneNumber : ValueObject
    {
        /// <summary>
        /// Форматированный номер телефона.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PhoneNumber"/>.
        /// </summary>
        /// <param name="value">Строка телефонного номера. Ожидается российский формат.</param>
        /// <exception cref="DomainException">Выбрасывается при неверном формате.</exception>
        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Номер телефона не может быть пустым.");

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length == 11 && (digitsOnly.StartsWith("8") || digitsOnly.StartsWith("7")))
                digitsOnly = string.Concat("7", digitsOnly.AsSpan(1));
            else if (digitsOnly.Length == 10)
                digitsOnly = string.Concat("7", digitsOnly);

            if (digitsOnly.Length != 11 || !digitsOnly.StartsWith("7"))
                throw new DomainException("Некорректный номер телефона. Ожидается российский номер.");

            Value = $"+{digitsOnly[0]} ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)} {digitsOnly.Substring(7, 2)} {digitsOnly.Substring(9, 2)}";
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    }
}