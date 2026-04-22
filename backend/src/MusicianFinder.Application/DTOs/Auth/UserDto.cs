namespace MusicianFinder.Application.DTOs.Auth
{
    /// <summary>
    /// DTO пользователя.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Создан ли профиль.
        /// </summary>
        public bool ProfileCreated { get; set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}