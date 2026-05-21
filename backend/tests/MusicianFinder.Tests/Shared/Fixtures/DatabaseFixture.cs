using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Core.Mapping;
using MusicianFinder.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace MusicianFinder.Tests.Shared.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private IServiceProvider _serviceProvider = null!;

        public string ConnectionString { get; private set; } = string.Empty;
        public AppDbContext DbContext { get; private set; } = null!;
        public IMapper Mapper { get; private set; } = null!;

        public DatabaseFixture()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("musicianfinder_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
            ConnectionString = _postgresContainer.GetConnectionString();
            if (!ConnectionString.Contains("Include Error Detail"))
                ConnectionString += ";Include Error Detail=true";

            var services = new ServiceCollection();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(ConnectionString));

            _serviceProvider = services.BuildServiceProvider();
            Mapper = _serviceProvider.GetRequiredService<IMapper>();
            DbContext = _serviceProvider.GetRequiredService<AppDbContext>();

            await DbContext.Database.MigrateAsync();
            await ResetDatabaseAsync();
        }

        /// <summary>Создаёт новый контекст, изолированный от предыдущих операций.</summary>
        public AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(ConnectionString);
            return new AppDbContext(optionsBuilder.Options);
        }

        public async Task ResetDatabaseAsync()
        {
            await DbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");
            foreach (var table in new[]
            {
                "EventRegistration", "Event", "Notification", "CollaborationSuggestion",
                "Favorite", "PortfolioItem", "MusicianProfile", "User", "EmailVerificationCode"
            })
            {
                await DbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
            }
            await DbContext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
        }

        public async Task DisposeAsync()
        {
            if (DbContext != null)
                await DbContext.DisposeAsync();
            await _postgresContainer.DisposeAsync();
        }
    }
}