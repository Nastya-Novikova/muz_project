using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Infrastructure.Idempotency;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="IdempotencyRecord"/>.
    /// </summary>
    public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecord");
            builder.HasKey(r => r.Key);
            builder.Property(r => r.Key).HasMaxLength(200);
            builder.Property(r => r.RequestHash).IsRequired().HasMaxLength(500);
            builder.Property(r => r.Response);
            builder.Property(r => r.Status).IsRequired().HasMaxLength(50);
            builder.Property(r => r.CreatedAt).IsRequired();
        }
    }
}