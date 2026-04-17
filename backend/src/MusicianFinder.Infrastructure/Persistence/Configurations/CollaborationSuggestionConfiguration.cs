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
    /// Конфигурация сущности <see cref="CollaborationSuggestion"/>.
    /// </summary>
    public class CollaborationSuggestionConfiguration : IEntityTypeConfiguration<CollaborationSuggestion>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CollaborationSuggestion> builder)
        {
            builder.ToTable("CollaborationSuggestion");
            builder.HasKey(cs => cs.Id);
            builder.Property(cs => cs.Message).HasMaxLength(500);
            builder.Property(cs => cs.Status).IsRequired().HasMaxLength(20);
            builder.Property(cs => cs.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(cs => cs.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(cs => cs.FromProfile)
                .WithMany()
                .HasForeignKey(cs => cs.FromProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.ToProfile)
                .WithMany()
                .HasForeignKey(cs => cs.ToProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}