using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MusicianFinder.Application.Features.VkIntegration.DTOs
{
    /// <summary>
    /// Ответ VK API с токеном.
    /// </summary>
    public class VkTokenResponse
    {
        /// <summary>
        /// Токен доступа.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// Ошибка.
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Описание ошибки.
        /// </summary>
        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }
}