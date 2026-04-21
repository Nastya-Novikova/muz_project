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
    /// Конфигурация сущности <see cref="CollaborationGoal"/>.
    /// </summary>
    public class CollaborationGoalConfiguration : IEntityTypeConfiguration<CollaborationGoal>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CollaborationGoal> builder)
        {
            builder.ToTable("CollaborationGoal");
            builder.HasKey(cg => cg.Id);
            builder.Property(cg => cg.Name).IsRequired().HasMaxLength(50);
            builder.Property(cg => cg.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new { Id = 1, Name = "band", LocalizedName = "Ищу участников в группу" },
                new { Id = 2, Name = "session", LocalizedName = "Готов(а) к сессионной работе" },
                new { Id = 3, Name = "collaboration", LocalizedName = "Открыт(а) к совместным проектам" },
                new { Id = 4, Name = "producer", LocalizedName = "Ищу продюсера" },
                new { Id = 5, Name = "artist", LocalizedName = "Ищу исполнителя для песен" }
            );
        }
    }
}