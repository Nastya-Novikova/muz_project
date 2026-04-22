using Microsoft.EntityFrameworkCore;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.API.Extensions
{
    /// <summary>
    /// Методы расширения для конвейера приложения.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Применяет миграции базы данных при запуске.
        /// </summary>
        /// <param name="app">Экземпляр <see cref="IApplicationBuilder"/>.</param>
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MusicianFinderDbContext>();
            dbContext.Database.Migrate();
        }
    }
}