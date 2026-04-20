using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="Event"/>.
    /// </summary>
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Address).HasMaxLength(200);
            builder.Property(e => e.Status).HasConversion<string>().HasDefaultValue(EventStatus.Scheduled);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            //builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasOne(e => e.Region)
                .WithMany()
                .HasForeignKey(e => e.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CreatorProfile)
                .WithMany()
                .HasForeignKey(e => e.CreatorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Metadata.FindNavigation(nameof(Event.Registrations))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}