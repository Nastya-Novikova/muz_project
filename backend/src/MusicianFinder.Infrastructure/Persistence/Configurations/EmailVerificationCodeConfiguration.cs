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
    /// Конфигурация сущности <see cref="EmailVerificationCode"/>.
    /// </summary>
    public class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
        {
            builder.ToTable("EmailVerificationCode");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(6);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasIndex(e => new { e.Email, e.Code, e.IsUsed });
        }
    }
}