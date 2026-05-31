using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class EmailServiceTests : TestBase, IAsyncLifetime
    {
        private readonly IContainer _mailHog;
        private IConfiguration _configuration = null!;
        private EmailService _service = null!;
        private HttpClient _httpClient = null!;

        public EmailServiceTests(ITestOutputHelper output) : base(output)
        {
            _mailHog = new ContainerBuilder()
                .WithImage("mailhog/mailhog:latest")
                .WithPortBinding(1025, true)   // SMTP
                .WithPortBinding(8025, true)   // HTTP API
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilHttpRequestIsSucceeded(r =>
                            r.ForPort(8025).ForPath("/api/v2/messages")))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _mailHog.StartAsync();

            var smtpPort = _mailHog.GetMappedPublicPort(1025);
            var apiPort = _mailHog.GetMappedPublicPort(8025);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["EmailSettings:SmtpServer"] = _mailHog.Hostname,
                ["EmailSettings:SmtpPort"] = smtpPort.ToString(),
                ["EmailSettings:SenderEmail"] = "noreply@musicianfinder.local",
                ["EmailSettings:SenderName"] = "Test Sender",
                ["EmailSettings:SmtpUsername"] = "",
                ["EmailSettings:SmtpPassword"] = ""
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var logger = LoggerFactory.Create(b => b.AddConsole())
                .CreateLogger<EmailService>();
            _service = new EmailService(_configuration, logger);

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://{_mailHog.Hostname}:{apiPort}");
        }

        public async Task DisposeAsync()
        {
            _httpClient?.Dispose();
            await _mailHog.DisposeAsync();
        }

        [Fact(Skip = "EmailService uses StartTls, MailHog does not support it on port 1025")]
        public async Task SendVerificationCodeAsync_ValidEmail_SendsEmail()
        {
            var email = "test@example.com";
            var code = "123456";

            await _service.SendVerificationCodeAsync(email, code);

            var response = await _httpClient.GetAsync("/api/v2/messages");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(email).And.Contain(code);
        }

        [Fact(Skip = "EmailService uses StartTls, MailHog does not support it on port 1025")]
        public async Task SendVerificationCodeAsync_SmtpUnavailable_ThrowsException()
        {
            await _mailHog.StopAsync();

            var serviceWithDeadSmtp = new EmailService(_configuration,
                LoggerFactory.Create(b => b.AddConsole()).CreateLogger<EmailService>());

            Func<Task> act = async () =>
                await serviceWithDeadSmtp.SendVerificationCodeAsync("fail@example.com", "000000");

            await act.Should().ThrowAsync<Exception>();
        }
    }
}