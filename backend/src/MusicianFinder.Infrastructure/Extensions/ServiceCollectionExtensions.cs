using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Infrastructure.Idempotency;
using MusicianFinder.Infrastructure.Outbox;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Infrastructure.Services;

namespace MusicianFinder.Infrastructure.Extensions
{
    /// <summary>
    /// Регистрация всех зависимостей слоя Infrastructure.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет службы Infrastructure в DI-контейнер.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Write-репозитории
            services.AddScoped<IMusicianProfileRepository, MusicianProfileRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICollaborationSuggestionRepository, CollaborationSuggestionRepository>();

            // Read-репозитории
            services.AddScoped<IProfileReadRepository, ProfileReadRepository>();
            services.AddScoped<IEventReadRepository, EventReadRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<ICollaborationSuggestionReadRepository, CollaborationSuggestionReadRepository>();
            services.AddScoped<IFavoriteReadRepository, FavoriteReadRepository>();
            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<IReferenceDataReadRepository, ReferenceDataReadRepository>();

            // Сервисы
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOutboxWriter, OutboxWriter>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorage, MinioFileStorage>();
            services.AddScoped<IVkService, VkService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddSingleton<IIntegrationEventTypeRegistry, IntegrationEventTypeRegistry>();
            services.AddSingleton<IExternalBusPublisher, ExternalBusPublisher>();
            services.AddScoped<IIdempotencyStore, DatabaseIdempotencyStore>();

            // Фоновые процессы
            services.AddHostedService<OutboxProcessor>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}