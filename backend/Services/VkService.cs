using System.Text.Json;
using backend.Models.Common;
using backend.Models.DTOs.Vk;
using backend.Services.Interfaces;
using backend.Models.Repositories.Interfaces;

namespace backend.Services;

public class VkService : IVkService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<VkService> _logger;

    public VkService(IProfileRepository profileRepository, IUnitOfWork unitOfWork, IConfiguration config, ILogger<VkService> logger)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _config = config;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Привязать VK аккаунт к профилю пользователя
    /// </summary>
    public async Task<Result> ConnectVkAsync(Guid userId, string code, string codeVerifier, string deviceId)
    {
        // 1. Находим профиль пользователя
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile == null)
            return Result.Failure("Profile not found");

        // 2. Проверяем, не привязан ли уже VK
        if (!string.IsNullOrEmpty(profile.VkUserId))
            return Result.Failure("VK already connected to this profile");

        // 3. Обмениваем code на vk_user_id
        var vkUserId = await ExchangeCodeAsync(code, codeVerifier, deviceId);
        if (!vkUserId.HasValue)
            return Result.Failure("Failed to get VK user ID");

        // 4. Сохраняем VkUserId в профиль
        profile.VkUserId = vkUserId.Value.ToString();
        await _profileRepository.UpdateAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} connected VK account {VkUserId}", userId, vkUserId);

        return Result.Success();
    }

    public async Task<long?> ExchangeCodeAsync(string code, string codeVerifier, string deviceId)
    {
        var appId = _config["VkSettings:AppId"];
        var redirectUri = _config["VkSettings:RedirectUri"];

        if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(redirectUri))
        {
            _logger.LogError("VkSettings: AppId or RedirectUri is missing");
            return null;
        }

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

        try
        {
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("VK exchange response: {Json}", json);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("VK returned error status: {StatusCode}", response.StatusCode);
                return null;
            }

            var tokenData = JsonSerializer.Deserialize<VkTokenResponse>(json);

            if (tokenData?.Error != null)
            {
                _logger.LogError("VK error: {Error} - {Description}", tokenData.Error, tokenData.ErrorDescription);
                return null;
            }

            return tokenData?.UserId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during VK code exchange");
            return null;
        }
    }

    public async Task<bool> SendNotificationAsync(Guid userId, string message)
    {
        var communityToken = _config["VkSettings:CommunityToken"];

        if (string.IsNullOrEmpty(communityToken))
        {
            _logger.LogError("Community token is not configured");
            return false;
        }

        // Находим профиль пользователя
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile == null)
        {
            _logger.LogError("Profile not found");
            return false;
        }

        if (string.IsNullOrEmpty(profile.VkUserId))
        {
            _logger.LogError("User has not connected VK account");
            return false;
        }

        var vkUserId = long.Parse(profile.VkUserId);

        // Согласно документации VK API, обязательные параметры:
        // - user_id (или peer_id) — кому отправляем
        // - random_id — уникальный идентификатор (защита от дублей)
        // - access_token — токен сообщества с правами messages
        // - v — версия API (минимум 5.131)
        var parameters = new Dictionary<string, string>
        {
            ["user_id"] = vkUserId.ToString(),
            ["message"] = message,
            ["random_id"] = new Random().Next().ToString(),
            ["access_token"] = communityToken,
            ["v"] = "5.131"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/messages.send")
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        try
        {
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("VK send message response: {Json}", json);

            if (json.Contains("\"response\":"))
            {
                _logger.LogInformation("Message sent successfully to user {VkUserId}", vkUserId);
                return true;
            }

            _logger.LogError("VK API error: {Json}", json);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send VK notification to user {VkUserId}", vkUserId);
            return false;
        }
    }
}