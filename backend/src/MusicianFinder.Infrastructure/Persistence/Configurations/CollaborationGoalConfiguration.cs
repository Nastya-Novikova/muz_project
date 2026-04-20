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

            /*builder.HasData(
                new CollaborationGoal("band", "Ищу участников в группу"),
                new CollaborationGoal("session", "Готов(а) к сессионной работе"),
                new CollaborationGoal("collaboration", "Открыт(а) к совместным проектам"),
                new CollaborationGoal("producer", "Ищу продюсера"),
                new CollaborationGoal("artist", "Ищу исполнителя для песен")
            );*/
        }
    }
}