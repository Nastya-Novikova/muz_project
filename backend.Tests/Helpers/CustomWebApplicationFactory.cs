using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Testcontainers.PostgreSql;
using backend.Data;

namespace backend.Tests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly ILogger<CustomWebApplicationFactory> _logger;

    public CustomWebApplicationFactory()
    {
        // Отключаем Ryuk, так как Docker не может его скачать
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");

        _dbContainer = new PostgreSqlBuilder()
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
        "EventRegistrations", "Events", "Notifications", "CollaborationSuggestions",
        "Favorites", "PortfolioAudio", "PortfolioVideo", "PortfolioPhotos",
        "MusicianProfiles", "Users", "EmailVerificationCodes"
    };

        foreach (var table in tables)
        {
            await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
        }

        await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
    }
}