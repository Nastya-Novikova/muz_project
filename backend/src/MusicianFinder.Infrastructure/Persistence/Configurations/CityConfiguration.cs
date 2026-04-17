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
    /// Конфигурация сущности <see cref="City"/>.
    /// </summary>
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("City");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new City("Moscow", "Москва"),
                new City("Saint Petersburg", "Санкт-Петербург"),
                new City("Novosibirsk", "Новосибирск"),
                new City("Yekaterinburg", "Екатеринбург"),
                new City("Kazan", "Казань")
            );
        }
    }
}