using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.Auth.DTOs
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