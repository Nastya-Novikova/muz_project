using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using System.Text.Json;
using MusicianFinder.Application.Features.VkIntegration.DTOs;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для взаимодействия с API ВКонтакте.
    /// </summary>
    public class VkService : IVkService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VkService> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="VkService"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="logger">Логгер.</param>
        public VkService(IProfileRepository profileRepository, IConfiguration configuration, ILogger<VkService> logger)
        {
            _profileRepository = profileRepository;
            _httpClient = new HttpClient();
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ConnectVkAsync(Guid userId, string code, string codeVerifier, string deviceId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new InvalidOperationException("Профиль не найден.");

            var vkUserId = await ExchangeCodeAsync(code, codeVerifier, deviceId);
            if (!vkUserId.HasValue)
                throw new InvalidOperationException("Не удалось получить идентификатор пользователя VK.");

            profile.SetVkUserId(vkUserId.Value.ToString());
            await _profileRepository.UpdateAsync(profile);
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
            var profile = await _profileRepository.GetByUserIdAsync(userId);
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
    }
}