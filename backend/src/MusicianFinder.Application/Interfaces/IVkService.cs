namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для взаимодействия с API ВКонтакте.
    /// </summary>
    public interface IVkService
    {
        /// <summary>
        /// Привязать аккаунт ВКонтакте к профилю пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="code">Код авторизации OAuth.</param>
        /// <param name="codeVerifier">Верификатор кода (PKCE).</param>
        /// <param name="deviceId">Идентификатор устройства.</param>
        Task ConnectVkAsync(Guid userId, string code, string codeVerifier, string deviceId);

        /// <summary>
        /// Обменять временный код на идентификатор пользователя ВКонтакте.
        /// </summary>
        /// <param name="code">Код авторизации.</param>
        /// <param name="codeVerifier">Верификатор кода.</param>
        /// <param name="deviceId">Идентификатор устройства.</param>
        /// <returns>Идентификатор пользователя VK или null в случае ошибки.</returns>
        Task<long?> ExchangeCodeAsync(string code, string codeVerifier, string deviceId);

        /// <summary>
        /// Отправить сообщение пользователю ВКонтакте.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя системы.</param>
        /// <param name="message">Текст сообщения.</param>
        /// <returns>true, если сообщение отправлено успешно.</returns>
        Task<bool> SendNotificationAsync(Guid userId, string message);
    }
}