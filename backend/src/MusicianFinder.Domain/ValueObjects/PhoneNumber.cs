using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Value Object, представляющий номер телефона.
    /// </summary>
    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex PhoneRegex = new(
            @"^\+?[1-9]\d{1,14}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Номер телефона в международном формате (E.164).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создаёт новый экземпляр <see cref="PhoneNumber"/>.
        /// </summary>
        /// <param name="value">Строка номера телефона.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если строка пуста или имеет неверный формат.</exception>
        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Номер телефона не может быть пустым.", nameof(value));

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11 && (digitsOnly.StartsWith("8") || digitsOnly.StartsWith("7")))
            {
                digitsOnly = "7" + digitsOnly.Substring(1);
            }
            else if (digitsOnly.Length == 10)
            {
                digitsOnly = "7" + digitsOnly;
            }

            if (digitsOnly.Length != 11 || !digitsOnly.StartsWith("7"))
                throw new ArgumentException("Некорректный номер телефона. Ожидается российский номер.", nameof(value));

            var formatted = $"+{digitsOnly[0]} ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)} {digitsOnly.Substring(7, 2)} {digitsOnly.Substring(9, 2)}";
            Value = formatted;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

        /// <inheritdoc />
        public bool Equals(PhoneNumber? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value;

        public static bool operator ==(PhoneNumber? left, PhoneNumber? right) => Equals(left, right);
        public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !Equals(left, right);

        /// <summary>
        /// Неявное преобразование из строки в <see cref="PhoneNumber"/>.
        /// </summary>
        public static implicit operator PhoneNumber(string value) => new(value);

        /// <summary>
        /// Неявное преобразование из <see cref="PhoneNumber"/> в строку.
        /// </summary>
        public static implicit operator string(PhoneNumber phone) => phone.Value;
    }
}
