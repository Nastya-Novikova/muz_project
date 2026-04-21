using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Value Object, представляющий email-адрес.
    /// </summary>
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Строковое представление email-адреса.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создаёт новый экземпляр <see cref="Email"/>.
        /// </summary>
        /// <param name="value">Строка email-адреса.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если строка пуста или имеет неверный формат.</exception>
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email не может быть пустым.", nameof(value));

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Некорректный формат email.", nameof(value));

            Value = value.ToLowerInvariant();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as Email);

        /// <inheritdoc />
        public bool Equals(Email? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value;

        public static bool operator ==(Email? left, Email? right) => Equals(left, right);
        public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

        /// <summary>
        /// Неявное преобразование из строки в <see cref="Email"/>.
        /// </summary>
        public static implicit operator Email(string value) => new(value);

        /// <summary>
        /// Неявное преобразование из <see cref="Email"/> в строку.
        /// </summary>
        public static implicit operator string(Email email) => email.Value;
    }
}