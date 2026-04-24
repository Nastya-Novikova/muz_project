namespace MusicianFinder.SharedKernel.Primitives
{
    /// <summary>
    /// Представляет результат операции, который может быть успешным или содержать ошибку.
    /// </summary>
    /// <typeparam name="T">Тип значения при успехе.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Значение (доступно только при успехе).
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string? Error { get; }

        /// <summary>
        /// Признак успешного завершения операции.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Признак неудачного завершения операции.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        private Result(string error)
        {
            Error = error;
            IsSuccess = false;
        }

        /// <summary>
        /// Создаёт успешный результат.
        /// </summary>
        /// <param name="value">Значение.</param>
        public static Result<T> Success(T value) => new(value);

        /// <summary>
        /// Создаёт неудачный результат.
        /// </summary>
        /// <param name="error">Сообщение об ошибке.</param>
        public static Result<T> Failure(string error) => new(error);
    }
}