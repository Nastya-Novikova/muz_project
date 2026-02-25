using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using backend.Models.Enums;

namespace backend.Data
{
    public class MusicianFinderDbContext : DbContext
    {
        public MusicianFinderDbContext(DbContextOptions<MusicianFinderDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<MusicalSpecialty> MusicalSpecialties => Set<MusicalSpecialty>();
        public DbSet<CollaborationGoal> CollaborationGoals => Set<CollaborationGoal>();
        public DbSet<EmailVerificationCode> EmailVerificationCodes => Set<EmailVerificationCode>();
        public DbSet<CollaborationSuggestion> CollaborationSuggestions => Set<CollaborationSuggestion>();
        public DbSet<PortfolioAudio> PortfolioAudio => Set<PortfolioAudio>();
        public DbSet<PortfolioVideo> PortfolioVideo => Set<PortfolioVideo>();
        public DbSet<PortfolioPhoto> PortfolioPhotos => Set<PortfolioPhoto>();
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Конфигурация User ===
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>().HasDefaultValue(UserRole.User);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasQueryFilter(u => !u.IsDeleted);

                // Связь один-к-одному с MusicianProfile
                entity.HasOne(u => u.MusicianProfile)
                    .WithOne()
                    .HasForeignKey<MusicianProfile>(p => p.Id);
            });

            // === Конфигурация Favorite ===
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("Favorites");
                entity.HasKey(f => new { f.UserId, f.ProfileId });

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Profile)
                    .WithMany()
                    .HasForeignKey(f => f.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // === Конфигурация MusicianProfile ===
            modelBuilder.Entity<MusicianProfile>(entity =>
            {
                entity.ToTable("MusicianProfiles");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Avatar).HasColumnType("bytea");
                entity.Property(p => p.Phone).HasMaxLength(20);
                entity.Property(p => p.Telegram).HasMaxLength(50);
                entity.Property(p => p.Experience).HasDefaultValue(0);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(p => p.ProfileType).HasConversion<string>();
                entity.Property(p => p.LookingFor).HasConversion<string>();

                entity.HasQueryFilter(p => !p.IsDeleted);

                // Связь с городом
                entity.HasOne(p => p.City)
                    .WithMany(/*c => c.Profiles*/)
                    .HasForeignKey(p => p.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                // === Many-to-many для предлагаемых жанров ===
                entity.HasMany(p => p.Genres)
                    .WithMany(g => g.Profiles)
                    .UsingEntity(j => j.ToTable("ProfileGenres"));

                // === Many-to-many для предлагаемых специализаций ===
                entity.HasMany(p => p.Specialties)
                    .WithMany(s => s.Profiles)
                    .UsingEntity(j => j.ToTable("ProfileSpecialties"));

                // === Many-to-many для целей сотрудничества ===
                entity.HasMany(p => p.CollaborationGoals)
                    .WithMany(cg => cg.Profiles)
                    .UsingEntity(j => j.ToTable("ProfileCollaborationGoals"));

                // === Many-to-many для искомых жанров ===
                entity.HasMany(p => p.DesiredGenres)
                    .WithMany(g => g.ProfilesLookingForThisGenre)
                    .UsingEntity(j => j.ToTable("ProfileDesiredGenres"));

                // === Many-to-many для искомых специализаций ===
                entity.HasMany(p => p.DesiredSpecialties)
                    .WithMany(s => s.ProfilesLookingForThisSpecialty)
                    .UsingEntity(j => j.ToTable("ProfileDesiredSpecialties"));

                // === Портфолио ===
                entity.HasMany(p => p.AudioFiles)
                    .WithOne(a => a.Profile)
                    .HasForeignKey(a => a.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.VideoFiles)
                    .WithOne(v => v.Profile)
                    .HasForeignKey(v => v.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Photos)
                    .WithOne(ph => ph.Profile)
                    .HasForeignKey(ph => ph.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // === Конфигурация City ===
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.Property(c => c.LocalizedName).IsRequired().HasMaxLength(50);
            });

            // === Конфигурация Genre ===
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genres");
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
                entity.Property(g => g.LocalizedName).IsRequired().HasMaxLength(50);
            });

            // === Конфигурация MusicalSpecialty ===
            modelBuilder.Entity<MusicalSpecialty>(entity =>
            {
                entity.ToTable("MusicalSpecialties");
                entity.HasKey(ms => ms.Id);
                entity.Property(ms => ms.Name).IsRequired().HasMaxLength(50);
                entity.Property(ms => ms.LocalizedName).IsRequired().HasMaxLength(50);
            });

            // === Конфигурация CollaborationGoal ===
            modelBuilder.Entity<CollaborationGoal>(entity =>
            {
                entity.ToTable("CollaborationGoals");
                entity.HasKey(cg => cg.Id);
                entity.Property(cg => cg.Name).IsRequired().HasMaxLength(50);
                entity.Property(cg => cg.LocalizedName).IsRequired().HasMaxLength(50);
            });

            // === Конфигурация EmailVerificationCode ===
            modelBuilder.Entity<EmailVerificationCode>(entity =>
            {
                entity.ToTable("EmailVerificationCodes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(6);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasIndex(e => new { e.Email, e.Code, e.IsUsed });
            });

            // === Конфигурация CollaborationSuggestion ===
            modelBuilder.Entity<CollaborationSuggestion>(entity =>
            {
                entity.ToTable("CollaborationSuggestions");
                entity.HasKey(cs => cs.Id);
                entity.Property(cs => cs.Message).HasMaxLength(500);
                entity.Property(cs => cs.Status).IsRequired().HasMaxLength(20);
                entity.Property(cs => cs.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(cs => cs.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(cs => cs.FromProfile)
                    .WithMany()
                    .HasForeignKey(cs => cs.FromProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cs => cs.ToProfile)
                    .WithMany()
                    .HasForeignKey(cs => cs.ToProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // === Конфигурация PortfolioAudio ===
            modelBuilder.Entity<PortfolioAudio>(entity =>
            {
                entity.ToTable("PortfolioAudio");
                entity.HasKey(pa => pa.Id);
                entity.Property(pa => pa.Title).HasMaxLength(100);
                entity.Property(pa => pa.Description).HasMaxLength(500);
                entity.Property(pa => pa.FileData).HasColumnType("bytea");
                entity.Property(pa => pa.MimeType).HasMaxLength(50);
                entity.Property(pa => pa.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // === Конфигурация PortfolioVideo ===
            modelBuilder.Entity<PortfolioVideo>(entity =>
            {
                entity.ToTable("PortfolioVideo");
                entity.HasKey(pv => pv.Id);
                entity.Property(pv => pv.Title).HasMaxLength(100);
                entity.Property(pv => pv.Description).HasMaxLength(500);
                entity.Property(pv => pv.FileData).HasColumnType("bytea");
                entity.Property(pv => pv.MimeType).HasMaxLength(50);
                entity.Property(pv => pv.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // === Конфигурация PortfolioPhoto ===
            modelBuilder.Entity<PortfolioPhoto>(entity =>
            {
                entity.ToTable("PortfolioPhotos");
                entity.HasKey(pp => pp.Id);
                entity.Property(pp => pp.Title).HasMaxLength(100);
                entity.Property(pp => pp.Description).HasMaxLength(500);
                entity.Property(pp => pp.FileData).HasColumnType("bytea");
                entity.Property(pp => pp.MimeType).HasMaxLength(50);
                entity.Property(pp => pp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
























            /*modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<MusicianProfile>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<MusicianProfile>().Property(p => p.Avatar).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioPhoto>().Property(p => p.FileData).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioAudio>().Property(a => a.FileData).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioVideo>().Property(v => v.FileData).HasColumnType("bytea");*/

            /*// === Many-to-many ===
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
                .HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId);

            // === One-to-many ===
            modelBuilder.Entity<PortfolioAudio>()
                .HasOne(a => a.Profile)
                .WithMany(p => p.AudioFiles)
                .HasForeignKey(a => a.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PortfolioVideo>()
                .HasOne(v => v.Profile)
                .WithMany(p => p.VideoFiles)
                .HasForeignKey(v => v.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PortfolioPhoto>()
                .HasOne(p => p.Profile)
                .WithMany(p => p.Photos)
                .HasForeignKey(p => p.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // === CollaborationSuggestion → MusicianProfile ===
            modelBuilder.Entity<CollaborationSuggestion>()
                .HasOne(s => s.FromProfile)
                .WithMany()
                .HasForeignKey(s => s.FromProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollaborationSuggestion>()
                .HasOne(s => s.ToProfile)
                .WithMany()
                .HasForeignKey(s => s.ToProfileId)
                .OnDelete(DeleteBehavior.Cascade);*/



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

            // Специальности
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

            // Цели
            modelBuilder.Entity<CollaborationGoal>().HasData(
                new CollaborationGoal { Id = 1, Name = "band", LocalizedName = "Ищу участников в группу" },
                new CollaborationGoal { Id = 2, Name = "session", LocalizedName = "Готов(а) к сессионной работе" },
                new CollaborationGoal { Id = 3, Name = "collaboration", LocalizedName = "Открыт(а) к совместным проектам" },
                new CollaborationGoal { Id = 4, Name = "producer", LocalizedName = "Ищу продюсера" },
                new CollaborationGoal { Id = 5, Name = "artist", LocalizedName = "Ищу исполнителя для песен" }
            );

            //this.SaveChangesAsync();
        }

        /*public static void EnsureDatabaseCreated(MusicianFinderDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");

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
        }*/
    }
}
