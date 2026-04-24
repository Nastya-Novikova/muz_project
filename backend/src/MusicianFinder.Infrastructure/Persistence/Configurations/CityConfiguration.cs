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
            builder.ToTable("Cities");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new City(new Guid("11111111-1111-1111-1111-111111111111"), "Moscow", "Москва"),
                new City(new Guid("22222222-2222-2222-2222-222222222222"), "Saint Petersburg", "Санкт-Петербург"),
                new City(new Guid("33333333-3333-3333-3333-333333333333"), "Novosibirsk", "Новосибирск"),
                new City(new Guid("44444444-4444-4444-4444-444444444444"), "Yekaterinburg", "Екатеринбург"),
                new City(new Guid("55555555-5555-5555-5555-555555555555"), "Kazan", "Казань")
            );
        }
    }
}