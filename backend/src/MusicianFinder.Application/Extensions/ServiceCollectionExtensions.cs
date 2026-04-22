using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MusicianFinder.Application.Behaviors;

namespace MusicianFinder.Application.Extensions
{
    /// <summary>
    /// Методы расширения для регистрации сервисов слоя Application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет сервисы слоя Application: MediatR, FluentValidation, AutoMapper и pipeline behaviors.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов с добавленными зависимостями Application.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Common.Mapping.MappingProfile).Assembly));
            return services;
        }
    }
}