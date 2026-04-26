using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Infrastructure.Outbox;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="DeadLetter"/>.
    /// </summary>
    public class DeadLetterConfiguration : IEntityTypeConfiguration<DeadLetter>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DeadLetter> builder)
        {
            builder.ToTable("DeadLetter");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedNever();
            builder.Property(d => d.OutboxMessageId).IsRequired();
            builder.Property(d => d.Error).IsRequired();
            builder.Property(d => d.MovedAt).IsRequired();
        }
    }
}