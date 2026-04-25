using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="City"/>.
    /// </summary>
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("City");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new City(1, "Moscow", "Москва"),
                new City(2, "Saint Petersburg", "Санкт-Петербург"),
                new City(3, "Novosibirsk", "Новосибирск"),
                new City(4, "Yekaterinburg", "Екатеринбург"),
                new City(5, "Kazan", "Казань")
            );
        }
    }
}