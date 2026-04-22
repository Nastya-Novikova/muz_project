using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="MusicalSpecialty"/>.
    /// </summary>
    public class MusicalSpecialtyConfiguration : IEntityTypeConfiguration<MusicalSpecialty>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MusicalSpecialty> builder)
        {
            builder.ToTable("MusicalSpecialty");
            builder.HasKey(ms => ms.Id);
            builder.Property(ms => ms.Name).IsRequired().HasMaxLength(50);
            builder.Property(ms => ms.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new { Id = 1, Name = "vocalist", LocalizedName = "Вокалист" },
                new { Id = 2, Name = "guitarist", LocalizedName = "Гитарист" },
                new { Id = 3, Name = "bassist", LocalizedName = "Бас-гитарист" },
                new { Id = 4, Name = "drummer", LocalizedName = "Ударник" },
                new { Id = 5, Name = "keyboardist", LocalizedName = "Клавишник" },
                new { Id = 6, Name = "composer", LocalizedName = "Композитор" },
                new { Id = 7, Name = "producer", LocalizedName = "Продюсер" },
                new { Id = 8, Name = "sound-engineer", LocalizedName = "Звукорежиссёр" },
                new { Id = 9, Name = "dj", LocalizedName = "Диджей" },
                new { Id = 10, Name = "violinist", LocalizedName = "Скрипач" }
            );
        }
    }
}