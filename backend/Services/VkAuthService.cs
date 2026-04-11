using System.Text.Json;
using backend.Models.Common;
using backend.Models.DTOs.Vk;
using backend.Services.Interfaces;
using backend.Models.Repositories.Interfaces;

namespace backend.Services;

public class VkAuthService : IVkAuthService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<VkAuthService> _logger;

    public VkAuthService(IProfileRepository profileRepository, IUnitOfWork unitOfWork, IConfiguration config, ILogger<VkAuthService> logger)
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
}