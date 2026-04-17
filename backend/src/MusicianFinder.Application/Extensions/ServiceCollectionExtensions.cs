using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;
using MusicianFinder.Application.Common.Behaviors;

namespace MusicianFinder.Application
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
            // Регистрация MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Регистрация валидаторов FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Регистрация pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // Регистрация AutoMapper (метод доступен в основном пакете, начиная с версии 13.0.0)
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Common.Mapping.MappingProfile).Assembly));

            return services;
        }
    }
}