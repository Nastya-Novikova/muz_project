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
            builder.ToTable("Regions");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new Region(new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Moscow Oblast", "Московская область"),
                new Region(new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Leningrad Oblast", "Ленинградская область"),
                new Region(new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Novosibirsk Oblast", "Новосибирская область"),
                new Region(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Sverdlovsk Oblast", "Свердловская область"),
                new Region(new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Tatarstan", "Татарстан")
            );
        }
    }
}