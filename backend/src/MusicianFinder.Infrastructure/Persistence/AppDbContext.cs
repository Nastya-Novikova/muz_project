using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Idempotency;
using MusicianFinder.Infrastructure.Outbox;
using MusicianFinder.Infrastructure.Persistence.Configurations;

namespace MusicianFinder.Infrastructure.Persistence
{
    /// <summary>
    /// Контекст базы данных для MusicianFinder.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <inheritdoc />
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>Города.</summary>
        public DbSet<City> Cities => Set<City>();
        /// <summary>Регионы.</summary>
        public DbSet<Region> Regions => Set<Region>();
        /// <summary>Жанры.</summary>
        public DbSet<Genre> Genres => Set<Genre>();
        /// <summary>Музыкальные специальности.</summary>
        public DbSet<MusicalSpecialty> MusicalSpecialties => Set<MusicalSpecialty>();
        /// <summary>Цели сотрудничества.</summary>
        public DbSet<CollaborationGoal> CollaborationGoals => Set<CollaborationGoal>();
        /// <summary>Профили музыкантов.</summary>
        public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();
        /// <summary>Пользователи.</summary>
        public DbSet<User> Users => Set<User>();
        /// <summary>Мероприятия.</summary>
        public DbSet<Event> Events => Set<Event>();
        /// <summary>Предложения о сотрудничестве.</summary>
        public DbSet<CollaborationSuggestion> CollaborationSuggestions => Set<CollaborationSuggestion>();
        /// <summary>Сообщения Outbox.</summary>
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        /// <summary>Записи идемпотентности.</summary>
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
        /// <summary>Сообщения Dead Letter Queue.</summary>
        public DbSet<DeadLetter> DeadLetters => Set<DeadLetter>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new MusicalSpecialtyConfiguration());
            modelBuilder.ApplyConfiguration(new CollaborationGoalConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new MusicianProfileConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new CollaborationSuggestionConfiguration());
            modelBuilder.ApplyConfiguration(new EmailVerificationCodeConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new DeadLetterConfiguration());
            modelBuilder.ApplyConfiguration(new IdempotencyRecordConfiguration());
        }
    }
}