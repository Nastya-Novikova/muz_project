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
                new MusicalSpecialty(1, "Arranger", "Аранжировщик"),
                new MusicalSpecialty(2, "Bassist", "Бас-гитарист"),
                new MusicalSpecialty(3, "Beatmaker", "Битмейкер"),
                new MusicalSpecialty(4, "Cellist", "Виолончелист"),
                new MusicalSpecialty(5, "Vocalist", "Вокалист"),
                new MusicalSpecialty(6, "Guitarist", "Гитарист"),
                new MusicalSpecialty(7, "DJ", "Диджей"),
                new MusicalSpecialty(8, "Conductor", "Дирижёр"),
                new MusicalSpecialty(9, "Sound Engineer", "Звукорежиссёр"),
                new MusicalSpecialty(10, "Keyboardist", "Клавишник"),
                new MusicalSpecialty(11, "Composer", "Композитор"),
                new MusicalSpecialty(12, "Concert Manager", "Концертный менеджер"),
                new MusicalSpecialty(13, "Pianist", "Пианист"),
                new MusicalSpecialty(14, "Producer", "Продюсер"),
                new MusicalSpecialty(15, "Rapper", "Рэпер"),
                new MusicalSpecialty(16, "Saxophonist", "Саксофонист"),
                new MusicalSpecialty(17, "Violinist", "Скрипач"),
                new MusicalSpecialty(18, "Trumpeter", "Трубач"),
                new MusicalSpecialty(19, "Drummer", "Ударник"),
                new MusicalSpecialty(20, "Flutist", "Флейтист")
            );
        }
    }
}