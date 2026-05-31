using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MusicianFinder.API;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Tests.Shared.Mocks;
using Testcontainers.PostgreSql;

namespace MusicianFinder.Tests.Shared.Fixtures
{
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
            builder.UseSetting("Jwt:Key", "SuperSecretTestKeyForTestingOnly123!");
            builder.UseSetting("Jwt:Issuer", "MusicianFinder");
            builder.UseSetting("Jwt:Audience", "MusicianFinder");
            builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());

            builder.ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                services.RemoveAll<IHostedService>();
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));

                services.RemoveAll<IEmailService>();
                services.AddScoped<IEmailService, MockEmailService>();

                services.RemoveAll<IVkService>();
                services.AddScoped<IVkService>(sp =>
                {
                    var mock = new MockVkService();
                    mock.SetExchangedUserId(123456789L);
                    return mock;
                });

                services.RemoveAll<IFileStorage>();
                services.AddScoped<IFileStorage, MockFileStorage>();

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IVerificationCodeService));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddScoped<IVerificationCodeService, MockVerificationCodeService>();

                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
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
            });
        }

        public string GetConnectionString() => _dbContainer.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await WaitForDatabaseAsync(_dbContainer.GetConnectionString(), TimeSpan.FromSeconds(30));
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static async Task WaitForDatabaseAsync(string connectionString, TimeSpan timeout)
        {
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < timeout)
            {
                try
                {
                    await using var conn = new Npgsql.NpgsqlConnection(connectionString);
                    await conn.OpenAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(500);
                }
            }
            throw new TimeoutException($"Database not ready after {timeout.TotalSeconds} s");
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await base.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");
            foreach (var table in new[]
            {
                "EventRegistration", "Event", "Notification", "CollaborationSuggestion",
                "Favorite", "PortfolioItem", "MusicianProfile", "User", "EmailVerificationCode"
            })
            {
                await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
            }
            await dbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync(Guid userId, string email, string role = "User")
        {
            var token = GenerateJwtToken(userId, email, role);
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await Task.FromResult(client);
        }

        private static string GenerateJwtToken(Guid userId, string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SuperSecretTestKeyForTestingOnly123!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim("email", email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = "MusicianFinder",
                Audience = "MusicianFinder",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // В конец класса CustomWebApplicationFactory добавляем:

        /// <summary>
        /// Установить флаг ошибки сохранения в MockFileStorage.
        /// </summary>
        public void SetMockFileStorageThrow(bool shouldThrow)
        {
            using var scope = Services.CreateScope();
            var mock = scope.ServiceProvider.GetService<IFileStorage>() as MockFileStorage;
            if (mock != null)
                mock.ShouldThrowOnSave = shouldThrow;
        }

        /// <summary>
        /// Установить возвращаемый exchangedUserId в MockVkService (null = ошибка обмена).
        /// </summary>
        public void SetMockVkServiceExchangedUserId(long? userId)
        {
            using var scope = Services.CreateScope();
            var mock = scope.ServiceProvider.GetService<IVkService>() as MockVkService;
            if (mock != null)
                mock.SetExchangedUserId(userId);
        }
    }
}