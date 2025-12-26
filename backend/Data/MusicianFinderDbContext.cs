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

            //modelBuilder.Entity<User>().Property(u => u.Avatar).HasColumnType("bytea");
            modelBuilder.Entity<MusicianProfile>().Property(p => p.Avatar).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioPhoto>().Property(p => p.FileData).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioAudio>().Property(a => a.FileData).HasColumnType("bytea");
            modelBuilder.Entity<PortfolioVideo>().Property(v => v.FileData).HasColumnType("bytea");

            /*modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Genres)
                .WithMany() // ← без аргумента
                .UsingEntity("ProfileGenre");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Specialties)
                .WithMany() // ← без аргумента
                .UsingEntity("ProfileSpecialty");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.CollaborationGoals)
                .WithMany() // ← без аргумента
                .UsingEntity("ProfileCollaborationGoal");*/

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
                .WithMany() // ← без обратной навигации
                .HasForeignKey(p => p.CityId);

            // === One-to-many: портфолио → профиль ===
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

            // === CollaborationSuggestion → User (обе стороны, но без обратной коллекции в User) ===
            modelBuilder.Entity<CollaborationSuggestion>()
                .HasOne(s => s.FromUser)
                .WithMany() // ← User не имеет SentSuggestions
                .HasForeignKey(s => s.FromProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollaborationSuggestion>()
                .HasOne(s => s.ToUser)
                .WithMany() // ← User не имеет ReceivedSuggestions
                .HasForeignKey(s => s.ToProfileId)
                .OnDelete(DeleteBehavior.Cascade);



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

            /*// Пользователи
            var user1 = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "user1@example.com",
                //FullName = "Алексей Иванов",
                //ProfileCompleted = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                FavoriteProfileIds = new List<Guid> { Guid.Parse("33333333-3333-3333-3333-333333333333") }
            };

            var user2 = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "user2@example.com",
                //FullName = "Мария Петрова",
                //ProfileCompleted = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-9),
                FavoriteProfileIds = new List<Guid> { Guid.Parse("44444444-4444-4444-4444-444444444444") }
            };

            var user3 = new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Email = "user3@example.com",
                //FullName = "Иван Сидоров",
                //ProfileCompleted = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-8),
                FavoriteProfileIds = new List<Guid>()
            };*/

            /*modelBuilder.Entity<User>().HasData(user1, user2, user3);

            // Профили
            var profile1 = new MusicianProfile
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                //UserId = user1.Id,
                FullName = "Алексей Иванов",
                CityId = 1,
                Age = 28,
                Experience = 7,
                Description = "Профессиональный гитарист, ищу группу для выступлений.",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-10),
            };

            var profile2 = new MusicianProfile
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                //UserId = user2.Id,
                FullName = "Мария Петрова",
                CityId = 2,
                Age = 25,
                Experience = 5,
                Description = "Вокалистка, участвую в джазовых проектах.",
                CreatedAt = DateTime.UtcNow.AddMinutes(-9),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-9)
            };

            var profile3 = new MusicianProfile
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                //UserId = user3.Id,
                FullName = "Иван Сидоров",
                CityId = 3,
                Age = 30,
                Experience = 10,
                Description = "Бас-гитарист, открыт к совместным проектам.",
                CreatedAt = DateTime.UtcNow.AddMinutes(-8),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-8)
            };

            modelBuilder.Entity<MusicianProfile>().HasData(profile1, profile2, profile3);

            // Связи: Profile ←→ Genre
            modelBuilder.Entity("ProfileGenre").HasData(
                new { GenresId = 2, ProfilesId = profile1.Id },
                new { GenresId = 1, ProfilesId = profile2.Id },
                new { GenresId = 2, ProfilesId = profile3.Id }
            );

            // Связи: Profile ←→ Specialty
            modelBuilder.Entity("ProfileSpecialty").HasData(
                new { ProfilesId = profile1.Id, SpecialtiesId = 2 },
                new { ProfilesId = profile2.Id, SpecialtiesId = 1 },
                new { ProfilesId = profile3.Id, SpecialtiesId = 3 }
            );

            // Связи: Profile ←→ Goal
            modelBuilder.Entity("ProfileCollaborationGoal").HasData(
                new { ProfilesId = profile1.Id, CollaborationGoalsId = 1 },
                new { ProfilesId = profile2.Id, CollaborationGoalsId = 3 },
                new { ProfilesId = profile3.Id, CollaborationGoalsId = 2 }
            );

            // Предложения
            modelBuilder.Entity<CollaborationSuggestion>().HasData(
                new CollaborationSuggestion
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    FromUserId = user1.Id,
                    ToUserId = user2.Id,
                    Message = "Привет! Слушал твои записи — классный голос. Хочу пригласить в новый проект.",
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
                },
                new CollaborationSuggestion
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    FromUserId = user2.Id,
                    ToUserId = user1.Id,
                    Message = "Спасибо! Давай обсудим детали.",
                    Status = "accepted",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-4),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-4)
                }
            );*/

            this.SaveChangesAsync();
            /*// Города
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
            );*/


            /*// Many-to-many
            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Genres)
                .WithMany(*//*g => g.Profiles*//*)
                .UsingEntity("ProfileGenre");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.Specialties)
                .WithMany(*//*s => s.Profiles*//*)
                .UsingEntity("ProfileSpecialty");

            modelBuilder.Entity<MusicianProfile>()
                .HasMany(p => p.CollaborationGoals)
                .WithMany(*//*g => g.Profiles*//*)
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

            modelBuilder.Entity<User>()
                .HasMany(*//*u => u.SentSuggestions*//*)
                .WithOne(s => s.FromUser)
                .HasForeignKey(s => s.FromUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(*//*u => u.ReceivedSuggestions*//*)
                .WithOne(s => s.ToUser)
                .HasForeignKey(s => s.ToUserId)
                .OnDelete(DeleteBehavior.Cascade);*/

            /*



                        // === Seed-данные пользователей ===
                        var user1 = new User
                        {
                            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                            Email = "user1@example.com",
                            FullName = "Алексей Иванов",
                            ProfileCompleted = true,
                            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                            FavoriteProfileIds = new List<Guid>
                {
                    Guid.Parse("33333333-3333-3333-3333-333333333333") // user2
                }
                        };

                        var user2 = new User
                        {
                            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                            Email = "user2@example.com",
                            FullName = "Мария Петрова",
                            ProfileCompleted = true,
                            CreatedAt = DateTime.UtcNow.AddMinutes(-9),
                            FavoriteProfileIds = new List<Guid>
                {
                    Guid.Parse("44444444-4444-4444-4444-444444444444") // user3
                }
                        };

                        var user3 = new User
                        {
                            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                            Email = "user3@example.com",
                            FullName = "Иван Сидоров",
                            ProfileCompleted = true,
                            CreatedAt = DateTime.UtcNow.AddMinutes(-8),
                            FavoriteProfileIds = new List<Guid>()
                        };

                        modelBuilder.Entity<User>().HasData(user1, user2, user3);

                        // === Seed-данные профилей ===
                        var profile1 = new MusicianProfile
                        {
                            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                            //UserId = user1.Id,
                            FullName = "Алексей Иванов",
                            CityId = 1, // Moscow
                            Age = 28,
                            Experience = 7,
                            Description = "Профессиональный гитарист, ищу группу для выступлений.",
                            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
                        };

                        var profile2 = new MusicianProfile
                        {
                            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                            //UserId = user2.Id,
                            FullName = "Мария Петрова",
                            CityId = 2, // Saint Petersburg
                            Age = 25,
                            Experience = 5,
                            Description = "Вокалистка, участвую в джазовых проектах.",
                            CreatedAt = DateTime.UtcNow.AddMinutes(-9),
                            UpdatedAt = DateTime.UtcNow.AddMinutes(-9)
                        };

                        var profile3 = new MusicianProfile
                        {
                            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                            //UserId = user3.Id,
                            FullName = "Иван Сидоров",
                            CityId = 3, // Novosibirsk
                            Age = 30,
                            Experience = 10,
                            Description = "Бас-гитарист, открыт к совместным проектам.",
                            CreatedAt = DateTime.UtcNow.AddMinutes(-8),
                            UpdatedAt = DateTime.UtcNow.AddMinutes(-8)
                        };

                        modelBuilder.Entity<MusicianProfile>().HasData(profile1, profile2, profile3);

                        // === Связи: Профиль — Жанры ===
                        modelBuilder.Entity("ProfileGenre").HasData(
                            new { GenresId = 2, ProfilesId = profile1.Id }, // rock
                            new { GenresId = 1, ProfilesId = profile2.Id }, // jazz
                            new { GenresId = 2, ProfilesId = profile3.Id }  // rock
                        );

                        // === Связи: Профиль — Специальности ===
                        modelBuilder.Entity("ProfileSpecialty").HasData(
                            new { ProfilesId = profile1.Id, SpecialtiesId = 2 }, // guitarist
                            new { ProfilesId = profile2.Id, SpecialtiesId = 1 }, // vocalist
                            new { ProfilesId = profile3.Id, SpecialtiesId = 3 }  // bassist
                        );

                        // === Связи: Профиль — Цели ===
                        modelBuilder.Entity("ProfileCollaborationGoal").HasData(
                            new { ProfilesId = profile1.Id, CollaborationGoalsId = 1 }, // ищу группу
                            new { ProfilesId = profile2.Id, CollaborationGoalsId = 3 }, // открыт к проектам
                            new { ProfilesId = profile3.Id, CollaborationGoalsId = 2 }  // сессионная работа
                        );

                        // === Предложения о сотрудничестве ===
                        modelBuilder.Entity<CollaborationSuggestion>().HasData(
                            new CollaborationSuggestion
                            {
                                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                                FromUserId = user1.Id,
                                ToUserId = user2.Id,
                                Message = "Привет! Слушал твои записи — классный голос. Хочу пригласить в новый проект.",
                                Status = "pending",
                                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                                UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
                            },
                            new CollaborationSuggestion
                            {
                                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                                FromUserId = user2.Id,
                                ToUserId = user1.Id,
                                Message = "Спасибо! Давай обсудим детали.",
                                Status = "accepted",
                                CreatedAt = DateTime.UtcNow.AddMinutes(-4),
                                UpdatedAt = DateTime.UtcNow.AddMinutes(-4)
                            }
                        );*/
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
