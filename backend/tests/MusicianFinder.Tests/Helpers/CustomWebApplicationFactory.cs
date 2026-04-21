using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.BackgroundServices;
using MusicianFinder.Infrastructure.Persistence;
using Npgsql;
using System.Text;
using Testcontainers.PostgreSql;

namespace MusicianFinder.Tests.Helpers;

/// <summary>
/// Фабрика для создания тестового веб-приложения с подменой БД на контейнер PostgreSQL и моками внешних сервисов.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");

        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("musicianfinder_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "SuperSecretTestKeyForTestingOnly123!",
                ["SmtpPassword"] = "raariggnzkiiarhg",
                ["CommunityToken"] = "vk1.a.CPQ2cCjoJMH-trY6G5eZVAv7PARSAbw3iT4q2nYjaB6fiLl8Tkn3Jj8dn1MLBFk_A8mrLeuKaJpUuQBEC9B6yryN3VJWSZ8dFiDzdCZ6Ejff56_zPJJBHiU5Orr7mAm80GzNOw0m8w8S9YCeQRVewVvZCbQYoTl-WUmJQ9DDd7cV81Mcw-TRu5oUWxA6kNyOulH4x9_taAzvFBwVEG3JNA",
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
            });
        });

        builder.ConfigureServices(services =>
        {
            // Удаляем фоновые сервисы
            services.RemoveAll<IHostedService>();
            services.RemoveAll<EventReminderBackgroundService>();

            // Подменяем БД
            services.RemoveAll<DbContextOptions<MusicianFinderDbContext>>();
            services.RemoveAll<MusicianFinderDbContext>();
            services.AddDbContext<MusicianFinderDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            // Моки для внешних сервисов
            services.RemoveAll<IEmailService>();
            services.AddScoped<IEmailService>(sp =>
            {
                var mock = new Mock<IEmailService>();
                mock.Setup(x => x.SendVerificationCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
                mock.Setup(x => x.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
                return mock.Object;
            });

            services.RemoveAll<IVkService>();
            services.AddScoped<IVkService>(sp =>
            {
                var mock = new Mock<IVkService>();
                mock.Setup(x => x.ConnectVkAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
                mock.Setup(x => x.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .ReturnsAsync(true);
                mock.Setup(x => x.ExchangeCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(123456789L);
                return mock.Object;
            });

            // === ЗАМЕНА JWT АУТЕНТИФИКАЦИИ НА ТЕСТОВУЮ ===
            services.RemoveAll<IAuthenticationSchemeProvider>();
            services.RemoveAll<IConfigureOptions<JwtBearerOptions>>();
            services.RemoveAll<IPostConfigureOptions<JwtBearerOptions>>();
            services.RemoveAll<JwtBearerHandler>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SuperSecretTestKeyForTestingOnly123!")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization();

            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        });

    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await WaitForDatabaseAsync(_dbContainer.GetConnectionString(), TimeSpan.FromSeconds(30));

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MusicianFinderDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static async Task WaitForDatabaseAsync(string connectionString, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                return;
            }
            catch
            {
                await Task.Delay(500);
            }
        }
        throw new TimeoutException($"Database not available after {timeout.TotalSeconds} seconds.");
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MusicianFinderDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");

        var tables = new[]
        {
            "EventRegistration",
            "Event",
            "Notification",
            "CollaborationSuggestion",
            "Favorite",
            "PortfolioAudio",
            "PortfolioVideo",
            "PortfolioPhoto",
            "MusicianProfile",
            "User",
            "EmailVerificationCode"
        };

        foreach (var table in tables)
        {
            await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
        }

        await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
    }
}