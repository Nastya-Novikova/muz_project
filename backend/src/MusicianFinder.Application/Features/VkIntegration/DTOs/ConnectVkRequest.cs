using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.VkIntegration.DTOs
{
    /// <summary>
    /// Запрос на привязку VK.
    /// </summary>
    public class ConnectVkRequest
    {
        /// <summary>
        /// Код авторизации.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Верификатор кода.
        /// </summary>
        public string CodeVerifier { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
    }
}