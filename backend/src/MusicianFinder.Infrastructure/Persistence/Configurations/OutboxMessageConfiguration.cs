using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Infrastructure.Outbox;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="OutboxMessage"/>.
    /// </summary>
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessage");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();
            builder.Property(m => m.EventName).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Version).IsRequired();
            builder.Property(m => m.Payload).IsRequired();
            builder.Property(m => m.CorrelationId).HasMaxLength(100);
            builder.Property(m => m.OccurredAt).IsRequired();
            builder.Property(m => m.ProcessedAt);
            builder.Property(m => m.NextAttemptAt).IsRequired();
            builder.Property(m => m.RetryCount).IsRequired();
            builder.HasIndex(m => new { m.ProcessedAt, m.NextAttemptAt });
        }
    }
}