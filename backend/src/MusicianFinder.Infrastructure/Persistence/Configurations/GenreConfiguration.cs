using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="Genre"/>.
    /// </summary>
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("Genre");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).ValueGeneratedNever();
            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.Property(g => g.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new Genre(1, "Alternative", "Альтернатива"),
                new Genre(2, "Blues", "Блюз"),
                new Genre(3, "Jazz", "Джаз"),
                new Genre(4, "Disco", "Диско"),
                new Genre(5, "Indie", "Инди"),
                new Genre(6, "Country", "Кантри"),
                new Genre(7, "Classical", "Классика"),
                new Genre(8, "Metal", "Метал"),
                new Genre(9, "Punk Rock", "Панк-рок"),
                new Genre(10, "Pop", "Поп"),
                new Genre(11, "Reggae", "Регги"),
                new Genre(12, "R&B", "Ритм-н-блюз"),
                new Genre(13, "Rock", "Рок"),
                new Genre(14, "Romance", "Романс"),
                new Genre(15, "Soul", "Соул"),
                new Genre(16, "Funk", "Фанк"),
                new Genre(17, "Folk", "Фолк"),
                new Genre(18, "Hip-Hop", "Хип-хоп"),
                new Genre(19, "Chanson", "Шансон"),
                new Genre(20, "Electronic", "Электроника")
            );
        }
    }
}