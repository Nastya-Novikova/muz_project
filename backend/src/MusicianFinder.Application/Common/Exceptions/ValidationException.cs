using FluentValidation.Results;

namespace MusicianFinder.Application.Common.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее об ошибках валидации запроса.
    /// </summary>
    public class ValidationException : System.Exception
    {
        /// <summary>
        /// Словарь ошибок валидации, где ключ — имя свойства, значение — массив сообщений.
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ValidationException"/>.
        /// </summary>
        public ValidationException() : base("Произошла одна или несколько ошибок валидации.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ValidationException"/> на основе списка ошибок FluentValidation.
        /// </summary>
        /// <param name="failures">Список ошибок валидации.</param>
        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}