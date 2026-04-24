using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.ValueObjects
{
    /// <summary>
    /// Идентификатор пользователя ВКонтакте (числовой).
    /// </summary>
    public sealed class VkUserId : ValueObject
    {
        /// <summary>
        /// Числовой идентификатор в виде строки.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр из строки.
        /// </summary>
        /// <param name="value">Строка, представляющая положительное число.</param>
        /// <exception cref="DomainException">Выбрасывается, если строка не является положительным числом.</exception>
        public VkUserId(string value)
        {
            if (!long.TryParse(value, out var id) || id <= 0)
                throw new DomainException("VK User ID должен быть положительным числом.");
            Value = id.ToString();
        }

        /// <summary>
        /// Инициализирует новый экземпляр из числа.
        /// </summary>
        /// <param name="id">Числовой идентификатор.</param>
        public VkUserId(long id) : this(id.ToString()) { }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    }
}