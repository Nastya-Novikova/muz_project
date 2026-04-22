using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.BackgroundServices;
using MusicianFinder.Infrastructure.Interceptors;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Services;

namespace MusicianFinder.Infrastructure.Extensions
{
    /// <summary>
    /// Методы расширения для регистрации сервисов слоя Infrastructure.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет сервисы слоя Infrastructure в контейнер DI.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция сервисов с добавленными зависимостями.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DispatchDomainEventsInterceptor>();

            services.AddDbContext<MusicianFinderDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(interceptor);
            });

            services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<MusicianFinderDbContext>());

            // Сервисы
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorage, MinioFileStorage>();
            services.AddScoped<IVkService, VkService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();

            services.AddHostedService<EventReminderBackgroundService>();

            return services;
        }
    }
}