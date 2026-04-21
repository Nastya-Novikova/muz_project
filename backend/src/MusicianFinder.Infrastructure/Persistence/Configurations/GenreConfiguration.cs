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

            builder.HasData(
                new { Id = 1, Name = "jazz", LocalizedName = "Джаз" },
                new { Id = 2, Name = "rock", LocalizedName = "Рок" },
                new { Id = 3, Name = "classical", LocalizedName = "Классика" },
                new { Id = 4, Name = "electronic", LocalizedName = "Электроника" },
                new { Id = 5, Name = "pop", LocalizedName = "Поп" },
                new { Id = 6, Name = "hip-hop", LocalizedName = "Хип-хоп" },
                new { Id = 7, Name = "metal", LocalizedName = "Метал" },
                new { Id = 8, Name = "blues", LocalizedName = "Блюз" }
            );
        }
    }
}