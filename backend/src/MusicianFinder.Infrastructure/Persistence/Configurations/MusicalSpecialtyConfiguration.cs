using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            /*builder.HasData(
                new MusicalSpecialty("vocalist", "Вокалист"),
                new MusicalSpecialty("guitarist", "Гитарист"),
                new MusicalSpecialty("bassist", "Бас-гитарист"),
                new MusicalSpecialty("drummer", "Ударник"),
                new MusicalSpecialty("keyboardist", "Клавишник"),
                new MusicalSpecialty("composer", "Композитор"),
                new MusicalSpecialty("producer", "Продюсер"),
                new MusicalSpecialty("sound-engineer", "Звукорежиссёр"),
                new MusicalSpecialty("dj", "Диджей"),
                new MusicalSpecialty("violinist", "Скрипач")
            );*/
        }
    }
}