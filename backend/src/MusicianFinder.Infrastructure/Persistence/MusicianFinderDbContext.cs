using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence
{
    /// <summary>
    /// Контекст базы данных для приложения MusicianFinder.
    /// </summary>
    public class MusicianFinderDbContext : DbContext, IReadDbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MusicianFinderDbContext"/>.
        /// </summary>
        /// <param name="options">Опции конфигурации контекста.</param>
        public MusicianFinderDbContext(DbContextOptions<MusicianFinderDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Пользователи.
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// Профили музыкантов.
        /// </summary>
        public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();

        /// <summary>
        /// Города.
        /// </summary>
        public DbSet<City> Cities => Set<City>();

        /// <summary>
        /// Регионы.
        /// </summary>
        public DbSet<Region> Regions => Set<Region>();

        /// <summary>
        /// Жанры.
        /// </summary>
        public DbSet<Genre> Genres => Set<Genre>();

        /// <summary>
        /// Музыкальные специальности.
        /// </summary>
        public DbSet<MusicalSpecialty> MusicalSpecialties => Set<MusicalSpecialty>();

        /// <summary>
        /// Цели сотрудничества.
        /// </summary>
        public DbSet<CollaborationGoal> CollaborationGoals => Set<CollaborationGoal>();

        /// <summary>
        /// Коды подтверждения email.
        /// </summary>
        public DbSet<EmailVerificationCode> EmailVerificationCodes => Set<EmailVerificationCode>();

        /// <summary>
        /// Предложения о сотрудничестве.
        /// </summary>
        public DbSet<CollaborationSuggestion> CollaborationSuggestions => Set<CollaborationSuggestion>();

        /// <summary>
        /// Избранные профили.
        /// </summary>
        public DbSet<Favorite> Favorites => Set<Favorite>();

        /// <summary>
        /// Мероприятия.
        /// </summary>
        public DbSet<Event> Events => Set<Event>();

        /// <summary>
        /// Регистрации на мероприятия.
        /// </summary>
        public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();

        /// <summary>
        /// Уведомления.
        /// </summary>
        public DbSet<Notification> Notifications => Set<Notification>();

        /// <inheritdoc />
        IQueryable<MusicianProfile> IReadDbContext.Profiles => MusicianProfiles;

        /// <inheritdoc />
        IQueryable<Event> IReadDbContext.Events => Events;

        /// <inheritdoc />
        IQueryable<CollaborationSuggestion> IReadDbContext.CollaborationSuggestions => CollaborationSuggestions;

        /// <inheritdoc />
        IQueryable<Notification> IReadDbContext.Notifications => Notifications;

        /// <inheritdoc />
        IQueryable<City> IReadDbContext.Cities => Cities;

        /// <inheritdoc />
        IQueryable<Region> IReadDbContext.Regions => Regions;

        /// <inheritdoc />
        IQueryable<Genre> IReadDbContext.Genres => Genres;

        /// <inheritdoc />
        IQueryable<MusicalSpecialty> IReadDbContext.Specialties => MusicalSpecialties;

        /// <inheritdoc />
        IQueryable<CollaborationGoal> IReadDbContext.CollaborationGoals => CollaborationGoals;

        /// <inheritdoc />
        IQueryable<User> IReadDbContext.Users => Users;

        /// <inheritdoc />
        IQueryable<EmailVerificationCode> IReadDbContext.EmailVerificationCodes => EmailVerificationCodes;

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}