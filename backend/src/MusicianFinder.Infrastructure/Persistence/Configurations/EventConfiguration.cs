using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

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
            builder.Property(e => e.Title)
                .HasConversion(title => title.Value, value => new EventTitle(value))
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(e => e.Description);
            builder.Property(e => e.ImageUrl);
            builder.Property(e => e.RegionId).IsRequired();
            builder.Property(e => e.CityId).IsRequired();
            builder.Property(e => e.Address).IsRequired().HasMaxLength(200);
            builder.Property(e => e.StartDateTime).HasColumnType("timestamp without time zone").IsRequired();
            builder.Property(e => e.EndDateTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.MaxParticipants).IsRequired();
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(EventStatus.Scheduled);
            builder.Property(e => e.CreatorProfileId).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.DeletedAt);

            builder.OwnsMany(e => e.Registrations, r =>
            {
                r.ToTable("EventRegistration");
                r.WithOwner().HasForeignKey("EventId");
                r.HasKey("EventId", "ProfileId");
                r.Property(x => x.RegisteredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.Ignore(e => e.DomainEvents);
        }
    }
}