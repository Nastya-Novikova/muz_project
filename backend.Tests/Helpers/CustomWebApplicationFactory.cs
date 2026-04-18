using backend.Data;
using backend.Models.Common;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;

namespace backend.Tests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly ILogger<CustomWebApplicationFactory> _logger;

    public CustomWebApplicationFactory()
    {
        // Отключаем Ryuk, так как Docker не может его скачать
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");

        _dbContainer = new PostgreSqlBuilder(PostgreSqlBuilder.PostgreSqlImage)
            .WithImage("postgres:16-alpine")
            .WithDatabase("musicianfinder_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)  // удалять контейнер после остановки
            .Build();

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<CustomWebApplicationFactory>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<MusicianFinderDbContext>>();
            services.RemoveAll<MusicianFinderDbContext>();
            services.AddDbContext<MusicianFinderDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

            // --- Моки внешних сервисов ---
            services.RemoveAll<IEmailService>();
            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(x => x.SendVerificationCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);
            emailMock.Setup(x => x.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);
            services.AddScoped(_ => emailMock.Object);

            services.RemoveAll<IVkService>();
            var vkMock = new Mock<IVkService>();
            vkMock.Setup(x => x.ConnectVkAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(Result.Success());
            vkMock.Setup(x => x.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                  .ReturnsAsync(true);
            services.AddScoped(_ => vkMock.Object);
        });
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Starting PostgreSQL container...");
        await _dbContainer.StartAsync();

        _logger.LogInformation("Waiting for database to become available...");
        await WaitForDatabaseAsync(_dbContainer.GetConnectionString(), TimeSpan.FromSeconds(30));

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MusicianFinderDbContext>();
        _logger.LogInformation("Applying migrations...");
        await dbContext.Database.MigrateAsync();
        _logger.LogInformation("Database ready.");
    }

    private async Task WaitForDatabaseAsync(string connectionString, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                _logger.LogInformation("Database connection established.");
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
        _logger.LogInformation("Stopping PostgreSQL container...");
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
#pragma warning disable EF1002
            await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
#pragma warning restore EF1002
        }

        await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
    }
}