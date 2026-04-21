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
    /// Конфигурация сущности <see cref="PortfolioVideo"/>.
    /// </summary>
    public class PortfolioVideoConfiguration : IEntityTypeConfiguration<PortfolioVideo>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<PortfolioVideo> builder)
        {
            builder.ToTable("PortfolioVideo");
            builder.HasKey(pv => pv.Id);
            builder.Property(pv => pv.Title).HasMaxLength(100);
            builder.Property(pv => pv.Description).HasMaxLength(500);
            builder.Property(pv => pv.MimeType).HasMaxLength(50);
            builder.Property(pv => pv.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}