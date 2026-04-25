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
            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new Region(1, "Moscow Oblast", "Московская область"),
                new Region(2, "Leningrad Oblast", "Ленинградская область"),
                new Region(3, "Novosibirsk Oblast", "Новосибирская область"),
                new Region(4, "Sverdlovsk Oblast", "Свердловская область"),
                new Region(5, "Tatarstan", "Татарстан")
            );
        }
    }
}