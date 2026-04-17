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
    /// Конфигурация сущности <see cref="EventRegistration"/>.
    /// </summary>
    public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<EventRegistration> builder)
        {
            builder.ToTable("EventRegistration");
            builder.HasKey(r => new { r.EventId, r.ProfileId });
            builder.HasIndex(r => new { r.EventId, r.ProfileId }).IsUnique();

            builder.HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.RegisteredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}