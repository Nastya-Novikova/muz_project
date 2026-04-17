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
    /// Конфигурация сущности <see cref="Notification"/>.
    /// </summary>
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Type).HasConversion<string>();
            builder.Property(n => n.EntityType).HasConversion<string>();
            builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
            builder.Property(n => n.Message).HasMaxLength(500);
            builder.Property(n => n.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(n => n.Profile)
                .WithMany()
                .HasForeignKey(n => n.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}