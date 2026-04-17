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
    /// Конфигурация сущности <see cref="PortfolioAudio"/>.
    /// </summary>
    public class PortfolioAudioConfiguration : IEntityTypeConfiguration<PortfolioAudio>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<PortfolioAudio> builder)
        {
            builder.ToTable("PortfolioAudio");
            builder.HasKey(pa => pa.Id);
            builder.Property(pa => pa.Title).HasMaxLength(100);
            builder.Property(pa => pa.Description).HasMaxLength(500);
            builder.Property(pa => pa.MimeType).HasMaxLength(50);
            builder.Property(pa => pa.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}