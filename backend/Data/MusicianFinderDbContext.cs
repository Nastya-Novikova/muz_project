using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class MusicianFinderDbContext : DbContext
    {
        public MusicianFinderDbContext(DbContextOptions<MusicianFinderDbContext> options)
        : base(options)
        {
        }
        public MusicianFinderDbContext()
        {
            //Database.EnsureCreated();
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MusicianProfile>();
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔸 Явно задайте имя таблицы
            modelBuilder.Entity<MusicianProfile>(entity =>
            {
                entity.ToTable("MusicianProfiles"); // ← Ключевое! Иначе имя может быть другим

                entity.HasKey(p => p.Id);

                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.City).HasMaxLength(50);
                entity.Property(p => p.Bio).HasMaxLength(500);
                entity.Property(p => p.LookingFor).HasMaxLength(50);

                // 🔸 Маппинг List<string> → text[]
                entity.Property(p => p.Instruments)
                      .HasConversion(
                          v => v.ToArray(),
                          v => v.ToList())
                      .HasColumnType("text[]");

                entity.Property(p => p.Genres)
                      .HasConversion(
                          v => v.ToArray(),
                          v => v.ToList())
                      .HasColumnType("text[]");
            });

            modelBuilder.Entity<MusicianProfile>().HasData(
           new MusicianProfile
           {
               Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
               FullName = "Alice Johnson",
               City = "Moscow",
               Bio = "Jazz vocalist looking for a band",
               LookingFor = "band",
               Instruments = new List<string> { "vocals" },
               Genres = new List<string> { "jazz", "soul" }
           },
           new MusicianProfile
           {
               Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
               FullName = "Bob Smith",
               City = "Saint Petersburg",
               Bio = "Guitarist, session musician",
               LookingFor = "session",
               Instruments = new List<string> { "guitar", "bass" },
               Genres = new List<string> { "rock", "blues" }
           },
           new MusicianProfile
           {
               Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
               FullName = "Clara Davis",
               City = "Novosibirsk",
               Bio = "Pianist and composer",
               LookingFor = "collaboration",
               Instruments = new List<string> { "piano", "synthesizer" },
               Genres = new List<string> { "classical", "electronic" }
           }
       );
        }
    }
}
