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
    /// Конфигурация сущности <see cref="Genre"/>.
    /// </summary>
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("Genre");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.Property(g => g.LocalizedName).IsRequired().HasMaxLength(50);

            /*builder.HasData(
                new Genre("jazz", "Джаз"),
                new Genre("rock", "Рок"),
                new Genre("classical", "Классика"),
                new Genre("electronic", "Электроника"),
                new Genre("pop", "Поп"),
                new Genre("hip-hop", "Хип-хоп"),
                new Genre("metal", "Метал"),
                new Genre("blues", "Блюз")
            );*/
        }
    }
}