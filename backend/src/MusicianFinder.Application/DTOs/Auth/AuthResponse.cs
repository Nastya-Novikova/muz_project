namespace MusicianFinder.Application.DTOs.Auth
{
    /// <summary>
    /// Ответ с данными аутентификации.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Успешность операции.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// JWT-токен.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Данные пользователя.
        /// </summary>
        public UserDto User { get; set; } = new();
    }
}