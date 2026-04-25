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
            builder.Property(cg => cg.Id).ValueGeneratedOnAdd();
            builder.Property(cg => cg.Name).IsRequired().HasMaxLength(50);
            builder.Property(cg => cg.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new CollaborationGoal(1, "band", "Ищу участников в группу"),
                new CollaborationGoal(2, "session", "Готов(а) к сессионной работе"),
                new CollaborationGoal(3, "collaboration", "Открыт(а) к совместным проектам"),
                new CollaborationGoal(4, "producer", "Ищу продюсера"),
                new CollaborationGoal(5, "artist", "Ищу исполнителя для песен")
            );
        }
    }
}