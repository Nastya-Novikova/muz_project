namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Базовый класс для объектов-значений, реализующий сравнение по структурному равенству.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Возвращает набор компонентов, определяющих равенство двух объектов-значений.
        /// </summary>
        /// <returns>Последовательность объектов для сравнения.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Оператор равенства.
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            if (left is null && right is null)
                return true;
            if (left is null || right is null)
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Оператор неравенства.
        /// </summary>
        public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
    }
}