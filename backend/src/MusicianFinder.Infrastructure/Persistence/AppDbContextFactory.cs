using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MusicianFinder.Infrastructure.Persistence
{
    /// <summary>
    /// Фабрика контекста базы данных для использования во время миграций (design-time).
    /// Не регистрирует бизнес-сервисы, которые требуют инфраструктуру, недоступную во время миграций.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// Создаёт новый экземпляр <see cref="AppDbContext"/> с конфигурацией для миграций.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        /// <returns>Новый контекст базы данных.</returns>
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=musicianfinder;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}