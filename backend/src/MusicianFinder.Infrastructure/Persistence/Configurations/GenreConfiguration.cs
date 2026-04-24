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
            builder.ToTable("Genres");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).ValueGeneratedNever();
            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.Property(g => g.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new Genre(new Guid("11111111-1111-1111-1111-111111111101"), "jazz", "Джаз"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111102"), "rock", "Рок"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111103"), "classical", "Классика"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111104"), "electronic", "Электроника"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111105"), "pop", "Поп"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111106"), "hip-hop", "Хип-хоп"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111107"), "metal", "Метал"),
                new Genre(new Guid("11111111-1111-1111-1111-111111111108"), "blues", "Блюз")
            );
        }
    }
}