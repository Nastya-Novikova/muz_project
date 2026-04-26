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
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
            builder.Property(s => s.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new MusicalSpecialty(1, "vocalist", "Вокалист"),
                new MusicalSpecialty(2, "guitarist", "Гитарист"),
                new MusicalSpecialty(3, "bassist", "Бас-гитарист"),
                new MusicalSpecialty(4, "drummer", "Ударник"),
                new MusicalSpecialty(5, "keyboardist", "Клавишник"),
                new MusicalSpecialty(6, "composer", "Композитор"),
                new MusicalSpecialty(7, "producer", "Продюсер"),
                new MusicalSpecialty(8, "sound-engineer", "Звукорежиссёр"),
                new MusicalSpecialty(9, "dj", "Диджей"),
                new MusicalSpecialty(10, "violinist", "Скрипач")
            );
        }
    }
}