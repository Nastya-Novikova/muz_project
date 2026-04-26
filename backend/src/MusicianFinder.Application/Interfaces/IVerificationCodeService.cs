namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для работы с кодами подтверждения email.
    /// </summary>
    public interface IVerificationCodeService
    {
        /// <summary>
        /// Генерирует и сохраняет код подтверждения для указанного email.
        /// </summary>
        /// <param name="email">Email адрес.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Сгенерированный код.</returns>
        Task<string> GenerateAndSaveCodeAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверяет, действителен ли переданный код для указанного email.
        /// При успехе код помечается как использованный.
        /// </summary>
        /// <param name="email">Email адрес.</param>
        /// <param name="code">Код подтверждения.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>true, если код корректен; иначе false.</returns>
        Task<bool> ValidateCodeAsync(string email, string code, CancellationToken cancellationToken = default);
    }
}