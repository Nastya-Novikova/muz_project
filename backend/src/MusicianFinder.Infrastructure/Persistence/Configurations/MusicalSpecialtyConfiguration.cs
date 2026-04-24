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
            builder.ToTable("MusicalSpecialties");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
            builder.Property(s => s.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111201"), "vocalist", "Вокалист"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111202"), "guitarist", "Гитарист"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111203"), "bassist", "Бас-гитарист"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111204"), "drummer", "Ударник"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111205"), "keyboardist", "Клавишник"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111206"), "composer", "Композитор"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111207"), "producer", "Продюсер"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111208"), "sound-engineer", "Звукорежиссёр"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111209"), "dj", "Диджей"),
                new MusicalSpecialty(new Guid("11111111-1111-1111-1111-111111111210"), "violinist", "Скрипач")
            );
        }
    }
}