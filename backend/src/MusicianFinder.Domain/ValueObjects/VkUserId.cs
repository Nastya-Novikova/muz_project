using System;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Value Object, представляющий идентификатор пользователя ВКонтакте.
    /// </summary>
    public sealed class VkUserId : IEquatable<VkUserId>
    {
        /// <summary>
        /// Идентификатор пользователя ВКонтакте (числовой).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="VkUserId"/>.
        /// </summary>
        /// <param name="value">Строковое представление идентификатора.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если строка пуста или не является положительным числом.</exception>
        public VkUserId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("VK User ID не может быть пустым.", nameof(value));

            if (!long.TryParse(value, out var id) || id <= 0)
                throw new ArgumentException("VK User ID должен быть положительным числом.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="VkUserId"/> из числового значения.
        /// </summary>
        /// <param name="id">Числовой идентификатор.</param>
        public VkUserId(long id) : this(id.ToString())
        {
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as VkUserId);

        /// <inheritdoc />
        public bool Equals(VkUserId? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value;

        /// <summary>
        /// Оператор равенства.
        /// </summary>
        public static bool operator ==(VkUserId? left, VkUserId? right) => Equals(left, right);

        /// <summary>
        /// Оператор неравенства.
        /// </summary>
        public static bool operator !=(VkUserId? left, VkUserId? right) => !Equals(left, right);

        /// <summary>
        /// Неявное преобразование из строки в <see cref="VkUserId"/>.
        /// </summary>
        public static implicit operator VkUserId(string value) => new(value);

        /// <summary>
        /// Неявное преобразование из числа в <see cref="VkUserId"/>.
        /// </summary>
        public static implicit operator VkUserId(long id) => new(id);

        /// <summary>
        /// Неявное преобразование из <see cref="VkUserId"/> в строку.
        /// </summary>
        public static implicit operator string(VkUserId vkId) => vkId.Value;

        /// <summary>
        /// Явное преобразование из <see cref="VkUserId"/> в число.
        /// </summary>
        public static explicit operator long(VkUserId vkId) => long.Parse(vkId.Value);
    }
}