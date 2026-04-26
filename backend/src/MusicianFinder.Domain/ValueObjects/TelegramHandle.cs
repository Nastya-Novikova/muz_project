using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Имя пользователя в Telegram (например, @username).
    /// </summary>
    public sealed class TelegramHandle : ValueObject
    {
        /// <summary>
        /// Имя пользователя без префикса @.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TelegramHandle"/>.
        /// </summary>
        /// <param name="handle">Telegram-имя. Допускается с @ или без. Длина от 5 до 32 символов.</param>
        /// <exception cref="DomainException">Выбрасывается при неверном формате.</exception>
        public TelegramHandle(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
                throw new DomainException("Telegram handle не может быть пустым.");

            var trimmed = handle.StartsWith("@") ? handle[1..] : handle;
            if (trimmed.Length < 5 || trimmed.Length > 32)
                throw new DomainException("Telegram handle должен содержать от 5 до 32 символов.");
            if (!trimmed.All(c => char.IsLetterOrDigit(c) || c == '_'))
                throw new DomainException("Telegram handle содержит недопустимые символы.");

            Value = trimmed.ToLowerInvariant();
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    }
}