using backend.Models.Common;

namespace backend.Services.Interfaces;

public interface IVkService
{
    /// <summary>
    /// Привязать VK аккаунт к профилю пользователя
    /// </summary>
    Task<Result> ConnectVkAsync(Guid userId, string code, string codeVerifier, string deviceId);

    /// <summary>
    /// Обменять временный код на user_id ВКонтакте
    /// </summary>
    Task<long?> ExchangeCodeAsync(string code, string codeVerifier, string deviceId);

    /// <summary>
    /// Отправить уведомление на user_id ВКонтакте
    /// </summary>
    Task<bool> SendNotificationAsync(Guid userId, string message);
}