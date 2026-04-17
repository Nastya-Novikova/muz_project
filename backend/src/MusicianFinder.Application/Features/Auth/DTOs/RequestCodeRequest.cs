using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Запрос на получение кода подтверждения.
    /// </summary>
    public class RequestCodeRequest
    {
        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}