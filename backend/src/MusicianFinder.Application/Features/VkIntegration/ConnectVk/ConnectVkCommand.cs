using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.VkIntegration.ConnectVk
{
    /// <summary>
    /// Команда для привязки аккаунта ВКонтакте.
    /// </summary>
    public class ConnectVkCommand : IRequest
    {
        /// <summary>
        /// Код авторизации.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Верификатор кода (PKCE).
        /// </summary>
        public string CodeVerifier { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
    }
}