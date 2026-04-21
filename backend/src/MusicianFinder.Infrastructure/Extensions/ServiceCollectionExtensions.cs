using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.BackgroundServices;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
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
            services.AddDbContext<MusicianFinderDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<DbContext>(sp => sp.GetRequiredService<MusicianFinderDbContext>());

            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IMusicalSpecialtyRepository, MusicalSpecialtyRepository>();
            services.AddScoped<ICollaborationGoalRepository, CollaborationGoalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<ICollaborationSuggestionRepository, CollaborationSuggestionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPortfolioAudioRepository, PortfolioAudioRepository>();
            services.AddScoped<IPortfolioVideoRepository, PortfolioVideoRepository>();
            services.AddScoped<IPortfolioPhotoRepository, PortfolioPhotoRepository>();
            services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();

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