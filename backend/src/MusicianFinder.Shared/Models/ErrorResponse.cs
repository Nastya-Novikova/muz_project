using System;

namespace MusicianFinder.Shared.Models
{
    /// <summary>
    /// Стандартизированный ответ с ошибкой.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Код ошибки.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Дополнительные данные (например, детали валидации).
        /// </summary>
        public object? Details { get; set; }

        /// <summary>
        /// Временная метка возникновения ошибки.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ErrorResponse"/>.
        /// </summary>
        /// <param name="code">Код ошибки.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="details">Дополнительные данные.</param>
        public ErrorResponse(string code, string message, object? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
            Timestamp = DateTime.UtcNow;
        }
    }
}