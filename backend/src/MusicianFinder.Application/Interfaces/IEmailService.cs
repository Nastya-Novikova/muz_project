namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для отправки электронных писем.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Отправить письмо с кодом подтверждения.
        /// </summary>
        /// <param name="toEmail">Email получателя.</param>
        /// <param name="code">Код подтверждения.</param>
        Task SendVerificationCodeAsync(string toEmail, string code);

        /// <summary>
        /// Отправить уведомление на email.
        /// </summary>
        /// <param name="toEmail">Email получателя.</param>
        /// <param name="subject">Тема письма.</param>
        /// <param name="body">Тело письма.</param>
        Task SendNotificationAsync(string toEmail, string subject, string body);
    }
}