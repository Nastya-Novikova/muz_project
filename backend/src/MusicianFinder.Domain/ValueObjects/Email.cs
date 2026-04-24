using System.Text.RegularExpressions;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Value Object для электронной почты.
    /// </summary>
    public sealed class Email : ValueObject
    {
        private static readonly Regex EmailRegex = new(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Строковое представление email (нижний регистр).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="Email"/>.
        /// </summary>
        /// <param name="value">Email-адрес.</param>
        /// <exception cref="DomainException">Выбрасывается при пустом или некорректном значении.</exception>
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email не может быть пустым.");
            if (!EmailRegex.IsMatch(value))
                throw new DomainException("Некорректный формат email.");
            Value = value.ToLowerInvariant();
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    }
}