using System.Text;
using backend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;

namespace backend.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _smtpServer = config["EmailSettings:SmtpServer"] ?? "localhost";
        _smtpPort = int.Parse(config["EmailSettings:SmtpPort"] ?? "1025");
        _senderEmail = config["EmailSettings:SenderEmail"] ?? "no-reply@example.com";
        _senderName = config["EmailSettings:SenderName"] ?? "MusicianFinder";
        _logger = logger;

        _logger.LogInformation("EmailService initialized with server: {Server}:{Port}",
            _smtpServer, _smtpPort);
    }

    public async Task SendVerificationCodeAsync(string toEmail, string code)
    {
        try
        {
            _logger.LogInformation("Attempting to send email to {Email} with code {Code}",
                toEmail, code);

            var message = new MimeMessage();

            // Отправитель и получатель
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));

            // Тема на английском
            message.Subject = "Your Verification Code - MusicianFinder";

            // Создаем тело письма с английским текстом
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Your Verification Code</h2>
                        <p style='font-size: 18px; margin: 20px 0;'>
                            Code: <strong style='color: #4a6fa5; font-size: 24px;'>{code}</strong>
                        </p>
                        <p>This code is valid for 10 minutes.</p>
                        <p>If you didn't request this code, please ignore this email.</p>
                        <hr style='margin: 20px 0;'>
                        <p style='color: #666; font-size: 12px;'>
                            MusicianFinder Team<br>
                            <a href='mailto:{_senderEmail}'>{_senderEmail}</a>
                        </p>
                    </div>",

                TextBody = $@"
                    Your Verification Code
                    ========================
                    
                    Code: {code}
                    
                    This code is valid for 10 minutes.
                    
                    If you didn't request this code, please ignore this email.
                    
                    ---
                    MusicianFinder Team
                    {_senderEmail}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Отправка через SMTP
            using var client = new SmtpClient();

            await client.ConnectAsync(_smtpServer, _smtpPort, false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}

/*using System.Text;
using backend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration; // Добавьте эту директиву using

namespace backend.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string? _smtpUsername; // Добавляем поля для аутентификации
    private readonly string? _smtpPassword;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _smtpServer = config["EmailSettings:SmtpServer"] ?? "localhost";
        _smtpPort = int.Parse(config["EmailSettings:SmtpPort"] ?? "1025");
        _senderEmail = config["EmailSettings:SenderEmail"] ?? "no-reply@example.com";
        _senderName = config["EmailSettings:SenderName"] ?? "MusicianFinder";
        _smtpUsername = config["EmailSettings:SmtpUsername"]; // Читаем логин
        _smtpPassword = config["EmailSettings:SmtpPassword"]; // Читаем пароль
        _logger = logger;

        _logger.LogInformation("EmailService initialized with server: {Server}:{Port}",
            _smtpServer, _smtpPort);
    }

    public async Task SendVerificationCodeAsync(string toEmail, string code)
    {
        try
        {
            _logger.LogInformation("Attempting to send email to {Email}", toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Your Verification Code";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<h2>Code: <strong>{code}</strong></h2><p>Valid for 10 minutes.</p>",
                TextBody = $"Code: {code}\nValid for 10 minutes."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Для реальных серверов:
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            // Аутентификация (используем сохраненные поля)
            if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
            {
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }
}*/