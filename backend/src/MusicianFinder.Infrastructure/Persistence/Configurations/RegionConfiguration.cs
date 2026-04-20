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

            /*builder.HasData(
                new Region("Moscow Oblast", "Московская область"),
                new Region("Leningrad Oblast", "Ленинградская область"),
                new Region("Novosibirsk Oblast", "Новосибирская область"),
                new Region("Sverdlovsk Oblast", "Свердловская область"),
                new Region("Tatarstan", "Татарстан")
            );*/
        }
    }
}