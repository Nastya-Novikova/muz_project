using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Auth.RequestCode
{
    /// <summary>
    /// Команда для запроса кода подтверждения на email.
    /// </summary>
    public class RequestCodeCommand : IRequest
    {
        /// <summary>
        /// Email для отправки кода.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}