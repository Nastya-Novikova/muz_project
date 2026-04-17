using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Auth.DTOs;

namespace MusicianFinder.Application.Features.Auth.Login
{
    /// <summary>
    /// Команда для входа/регистрации по коду подтверждения.
    /// </summary>
    public class LoginCommand : IRequest<AuthResponse>
    {
        /// <summary>
        /// Email пользователя.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код подтверждения.
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}