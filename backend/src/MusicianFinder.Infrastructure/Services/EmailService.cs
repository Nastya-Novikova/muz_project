using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для отправки электронных писем.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string? _smtpUsername;
        private readonly string? _smtpPassword;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EmailService"/>.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="logger">Логгер.</param>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? string.Empty;
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _senderEmail = configuration["EmailSettings:SenderEmail"] ?? string.Empty;
            _senderName = configuration["EmailSettings:SenderName"] ?? "MusicianFinder";
            _smtpUsername = configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = configuration["EmailSettings:SmtpPassword"];
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendVerificationCodeAsync(string toEmail, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Код подтверждения - MusicianFinder";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Код подтверждения</h2>
                        <p style='font-size: 18px; margin: 20px 0;'>
                            Код: <strong style='color: #4a6fa5; font-size: 24px;'>{code}</strong>
                        </p>
                        <p>Код действителен в течение 10 минут.</p>
                        <p>Если вы не запрашивали код, проигнорируйте это письмо.</p>
                    </div>",
                TextBody = $"Код подтверждения: {code}\nКод действителен 10 минут."
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Код подтверждения отправлен на {Email}", toEmail);
        }

        /// <inheritdoc />
        public async Task SendNotificationAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<div style='font-family: Arial, sans-serif;'><h3>{subject}</h3><p>{body}</p></div>",
                TextBody = $"{subject}\n\n{body}"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Уведомление отправлено на {Email}", toEmail);
        }
    }
}