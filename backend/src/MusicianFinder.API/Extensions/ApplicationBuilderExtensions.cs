using Microsoft.EntityFrameworkCore;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.API.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Применяет ожидающие миграции базы данных при запуске приложения.
        /// </summary>
        /// <param name="app">Построитель конвейера приложения.</param>
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
}