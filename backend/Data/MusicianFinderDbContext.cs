using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backend.Data
{
    public class MusicianFinderDbContext : DbContext
    {
        public MusicianFinderDbContext(DbContextOptions<MusicianFinderDbContext> options)
        : base(options)
        {
        }

        /*public DbSet<User> Users => Set<User>();
        public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<MusicalSpecialty> MusicalSpecialties => Set<MusicalSpecialty>();
        public DbSet<CollaborationGoal> CollaborationGoals => Set<CollaborationGoal>();
        public DbSet<EmailVerificationCode> EmailVerificationCodes => Set<EmailVerificationCode>();*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>();
            modelBuilder.Entity<MusicianProfile>();
            modelBuilder.Entity<City>();
            modelBuilder.Entity<Genre>();
            modelBuilder.Entity<MusicalSpecialty>();
            modelBuilder.Entity<CollaborationGoal>();
            modelBuilder.Entity<EmailVerificationCode>();
            modelBuilder.Entity<CollaborationSuggestion>();
            modelBuilder.Entity<PortfolioAudio>();
            modelBuilder.Entity<PortfolioVideo>();
            modelBuilder.Entity<PortfolioPhoto>();

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<MusicianProfile>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Avatar)
                      .HasColumnType("bytea");
            });

            modelBuilder.Entity<MusicianProfile>(entity =>
            {
                entity.Property(p => p.Avatar)
                      .HasColumnType("bytea");
            });

            modelBuilder.Entity<PortfolioPhoto>(entity =>
            {
                entity.Property(p => p.FileData).HasColumnType("bytea");
            });

            modelBuilder.Entity<PortfolioAudio>(entity =>
            {
                entity.Property(a => a.FileData).HasColumnType("bytea");
            });

            modelBuilder.Entity<PortfolioVideo>(entity =>
            {
                entity.Property(v => v.FileData).HasColumnType("bytea");
            });

            // === Seed-данные ===

            // Города
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Moscow", LocalizedName = "Москва" },
                new City { Id = 2, Name = "Saint Petersburg", LocalizedName = "Санкт-Петербург" },
                new City { Id = 3, Name = "Novosibirsk", LocalizedName = "Новосибирск" },
                new City { Id = 4, Name = "Yekaterinburg", LocalizedName = "Екатеринбург" },
                new City { Id = 5, Name = "Kazan", LocalizedName = "Казань" }
            );

            // Жанры
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "jazz", LocalizedName = "Джаз" },
                new Genre { Id = 2, Name = "rock", LocalizedName = "Рок" },
                new Genre { Id = 3, Name = "classical", LocalizedName = "Классика" },
                new Genre { Id = 4, Name = "electronic", LocalizedName = "Электроника" },
                new Genre { Id = 5, Name = "pop", LocalizedName = "Поп" },
                new Genre { Id = 6, Name = "hip-hop", LocalizedName = "Хип-хоп" },
                new Genre { Id = 7, Name = "metal", LocalizedName = "Метал" },
                new Genre { Id = 8, Name = "blues", LocalizedName = "Блюз" }
            );

            // Музыкальные специальности
            modelBuilder.Entity<MusicalSpecialty>().HasData(
                new MusicalSpecialty { Id = 1, Name = "vocalist", LocalizedName = "Вокалист" },
                new MusicalSpecialty { Id = 2, Name = "guitarist", LocalizedName = "Гитарист" },
                new MusicalSpecialty { Id = 3, Name = "bassist", LocalizedName = "Бас-гитарист" },
                new MusicalSpecialty { Id = 4, Name = "drummer", LocalizedName = "Ударник" },
                new MusicalSpecialty { Id = 5, Name = "keyboardist", LocalizedName = "Клавишник" },
                new MusicalSpecialty { Id = 6, Name = "composer", LocalizedName = "Композитор" },
                new MusicalSpecialty { Id = 7, Name = "producer", LocalizedName = "Продюсер" },
                new MusicalSpecialty { Id = 8, Name = "sound-engineer", LocalizedName = "Звукорежиссёр" },
                new MusicalSpecialty { Id = 9, Name = "dj", LocalizedName = "Диджей" },
                new MusicalSpecialty { Id = 10, Name = "violinist", LocalizedName = "Скрипач" }
            );

            // Цели сотрудничества
            modelBuilder.Entity<CollaborationGoal>().HasData(
                new CollaborationGoal { Id = 1, Name = "band", LocalizedName = "Ищу участников в группу" },
                new CollaborationGoal { Id = 2, Name = "session", LocalizedName = "Готов(а) к сессионной работе" },
                new CollaborationGoal { Id = 3, Name = "collaboration", LocalizedName = "Открыт(а) к совместным проектам" },
                new CollaborationGoal { Id = 4, Name = "producer", LocalizedName = "Ищу продюсера" },
                new CollaborationGoal { Id = 5, Name = "artist", LocalizedName = "Ищу исполнителя для песен" }
            );


            // Many-to-many
            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Genres)
                .WithMany(g => g.Profiles)
                .UsingEntity("ProfileGenre");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Specialties)
                .WithMany(s => s.Profiles)
                .UsingEntity("ProfileSpecialty");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.CollaborationGoals)
                .WithMany(g => g.Profiles)
                .UsingEntity("ProfileCollaborationGoal");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.AudioFiles)
                .WithOne(a => a.Profile)
                .HasForeignKey(a => a.ProfileId);

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.VideoFiles)
                .WithOne(v => v.Profile)
                .HasForeignKey(v => v.ProfileId);

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Photos)
                .WithOne(p => p.Profile)
                .HasForeignKey(p => p.ProfileId);
        }

        public static void EnsureDatabaseCreated(MusicianFinderDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Логируйте ошибку
                Console.WriteLine($"Migration error: {ex.Message}");

                // Попробуйте применить только отсутствующие миграции
                var appliedMigrations = context.Database.GetAppliedMigrations();
                var allMigrations = context.Database.GetMigrations();
                var pendingMigrations = allMigrations.Except(appliedMigrations);

                if (pendingMigrations.Any())
                {
                    var migrator = context.Database.GetService<IMigrator>();
                    foreach (var migration in pendingMigrations)
                    {
                        migrator.Migrate(migration);
                    }
                }
            }
        }
    }
}
