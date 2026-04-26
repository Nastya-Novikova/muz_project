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
                new Genre(1, "jazz", "Джаз"),
                new Genre(2, "rock", "Рок"),
                new Genre(3, "classical", "Классика"),
                new Genre(4, "electronic", "Электроника"),
                new Genre(5, "pop", "Поп"),
                new Genre(6, "hip-hop", "Хип-хоп"),
                new Genre(7, "metal", "Метал"),
                new Genre(8, "blues", "Блюз")
            );
        }
    }
}