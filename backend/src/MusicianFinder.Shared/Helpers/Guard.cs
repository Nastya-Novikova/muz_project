using System;

namespace MusicianFinder.Shared.Helpers
{
    /// <summary>
    /// Вспомогательный класс для проверки предусловий.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Проверяет, что аргумент не равен null.
        /// </summary>
        /// <typeparam name="T">Тип аргумента.</typeparam>
        /// <param name="value">Значение аргумента.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если значение null.</exception>
        public static void AgainstNull<T>(T? value, string paramName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Проверяет, что строка не является пустой или состоящей из пробелов.
        /// </summary>
        /// <param name="value">Значение строки.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если строка пуста или состоит из пробелов.</exception>
        public static void AgainstNullOrWhiteSpace(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Значение не может быть пустым или состоять из пробелов.", paramName);
        }

        /// <summary>
        /// Проверяет, что идентификатор не равен Guid.Empty.
        /// </summary>
        /// <param name="value">Значение идентификатора.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если идентификатор равен Guid.Empty.</exception>
        public static void AgainstEmptyGuid(Guid value, string paramName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Идентификатор не может быть пустым.", paramName);
        }

        /// <summary>
        /// Проверяет, что число больше или равно указанному минимуму.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="min">Минимальное допустимое значение.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение меньше минимума.</exception>
        public static void AgainstLessThan(int value, int min, string paramName)
        {
            if (value < min)
                throw new ArgumentOutOfRangeException(paramName, $"Значение не может быть меньше {min}.");
        }

        /// <summary>
        /// Проверяет, что число находится в указанном диапазоне.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="min">Минимальное допустимое значение.</param>
        /// <param name="max">Максимальное допустимое значение.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение вне диапазона.</exception>
        public static void AgainstOutOfRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, $"Значение должно быть между {min} и {max}.");
        }
    }
}