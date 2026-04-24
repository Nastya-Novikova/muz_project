namespace MusicianFinder.SharedKernel.Extensions
{
    /// <summary>
    /// Методы расширения для работы со строками.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Обрезает строку до указанной максимальной длины.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <param name="maxLength">Максимальная длина.</param>
        /// <returns>Обрезанная строка с добавлением многоточия, если было обрезание.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }

        /// <summary>
        /// Проверяет, является ли строка пустой или состоящей только из пробелов.
        /// </summary>
        /// <param name="value">Проверяемая строка.</param>
        /// <returns>true, если строка null, пуста или содержит только пробелы.</returns>
        public static bool IsNullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Возвращает значение по умолчанию, если строка пуста или состоит из пробелов.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <returns>Исходная строка или значение по умолчанию.</returns>
        public static string DefaultIfNullOrWhiteSpace(this string? value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}