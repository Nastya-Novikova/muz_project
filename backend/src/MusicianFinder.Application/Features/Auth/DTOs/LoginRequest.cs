using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Запрос на вход.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код подтверждения.
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}