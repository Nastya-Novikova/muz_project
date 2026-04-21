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
    /// Конфигурация сущности <see cref="Region"/>.
    /// </summary>
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable("Region");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new { Id = 1, Name = "Moscow Oblast", LocalizedName = "Московская область" },
                new { Id = 2, Name = "Leningrad Oblast", LocalizedName = "Ленинградская область" },
                new { Id = 3, Name = "Novosibirsk Oblast", LocalizedName = "Новосибирская область" },
                new { Id = 4, Name = "Sverdlovsk Oblast", LocalizedName = "Свердловская область" },
                new { Id = 5, Name = "Tatarstan", LocalizedName = "Татарстан" }
            );
        }
    }
}