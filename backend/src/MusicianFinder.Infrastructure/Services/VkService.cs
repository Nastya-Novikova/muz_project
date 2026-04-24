using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для взаимодействия с API ВКонтакте.
    /// </summary>
    public class VkService : IVkService
    {
        private readonly MusicianFinderDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VkService> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="VkService"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="logger">Логгер.</param>
        public VkService(
            MusicianFinderDbContext dbContext,
            IConfiguration configuration,
            ILogger<VkService> logger)
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ConnectVkAsync(Guid userId, string code, string codeVerifier, string deviceId)
        {
            var profile = await _dbContext.MusicianProfiles
                .FirstOrDefaultAsync(p => p.Id == userId && !p.IsDeleted);
            if (profile == null)
                throw new InvalidOperationException("Профиль не найден.");

            var vkUserId = await ExchangeCodeAsync(code, codeVerifier, deviceId);
            if (!vkUserId.HasValue)
                throw new InvalidOperationException("Не удалось получить идентификатор пользователя VK.");

            profile.SetVkUserId(vkUserId.Value.ToString());
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<long?> ExchangeCodeAsync(string code, string codeVerifier, string deviceId)
        {
            var appId = _configuration["VkSettings:AppId"];
            var redirectUri = _configuration["VkSettings:RedirectUri"];

            var parameters = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = appId!,
                ["code"] = code,
                ["code_verifier"] = codeVerifier,
                ["device_id"] = deviceId,
                ["redirect_uri"] = redirectUri!
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://id.vk.com/oauth2/auth")
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            var tokenData = JsonSerializer.Deserialize<VkTokenResponse>(json);
            if (tokenData?.Error != null)
            {
                _logger.LogError("VK error: {Error} - {Description}", tokenData.Error, tokenData.ErrorDescription);
                return null;
            }

            return tokenData?.UserId;
        }

        /// <inheritdoc />
        public async Task<bool> SendNotificationAsync(Guid userId, string message)
        {
            var profile = await _dbContext.MusicianProfiles
                .FirstOrDefaultAsync(p => p.Id == userId && !p.IsDeleted);
            if (profile == null || string.IsNullOrEmpty(profile.VkUserId))
                return false;

            var communityToken = _configuration["VkSettings:CommunityToken"];
            var parameters = new Dictionary<string, string>
            {
                ["user_id"] = profile.VkUserId,
                ["message"] = message,
                ["random_id"] = new Random().Next().ToString(),
                ["access_token"] = communityToken!,
                ["v"] = "5.131"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/messages.send")
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            return json.Contains("\"response\":");
        }

        private class VkTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("user_id")]
            public long UserId { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }

            [JsonPropertyName("error_description")]
            public string? ErrorDescription { get; set; }
        }
    }
}