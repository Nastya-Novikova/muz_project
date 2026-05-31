using System.Threading.Tasks;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Tests.Shared.Mocks
{
    /// <summary>
    /// Заглушка <see cref="IEmailService"/>, сохраняющая последние вызовы.
    /// </summary>
    public class MockEmailService : IEmailService
    {
        /// <summary>Последний email, на который отправлен код.</summary>
        public string? LastVerificationEmail { get; private set; }

        /// <summary>Последний отправленный код.</summary>
        public string? LastVerificationCode { get; private set; }

        /// <summary>Последнее отправленное уведомление (email, тема, тело).</summary>
        public (string ToEmail, string Subject, string Body)? LastNotification { get; private set; }

        public Task SendVerificationCodeAsync(string toEmail, string code)
        {
            LastVerificationEmail = toEmail;
            LastVerificationCode = code;
            return Task.CompletedTask;
        }

        public Task SendNotificationAsync(string toEmail, string subject, string body)
        {
            LastNotification = (toEmail, subject, body);
            return Task.CompletedTask;
        }
    }
}