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
    /// Конфигурация сущности <see cref="PortfolioPhoto"/>.
    /// </summary>
    public class PortfolioPhotoConfiguration : IEntityTypeConfiguration<PortfolioPhoto>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<PortfolioPhoto> builder)
        {
            builder.ToTable("PortfolioPhoto");
            builder.HasKey(pp => pp.Id);
            builder.Property(pp => pp.Title).HasMaxLength(100);
            builder.Property(pp => pp.Description).HasMaxLength(500);
            builder.Property(pp => pp.MimeType).HasMaxLength(50);
            builder.Property(pp => pp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}