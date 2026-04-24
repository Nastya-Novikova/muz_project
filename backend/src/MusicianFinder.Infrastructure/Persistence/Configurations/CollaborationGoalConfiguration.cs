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
            builder.ToTable("CollaborationGoals");
            builder.HasKey(cg => cg.Id);
            builder.Property(cg => cg.Id).ValueGeneratedNever();
            builder.Property(cg => cg.Name).IsRequired().HasMaxLength(50);
            builder.Property(cg => cg.LocalizedName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new CollaborationGoal(new Guid("11111111-1111-1111-1111-111111111301"), "band", "Ищу участников в группу"),
                new CollaborationGoal(new Guid("11111111-1111-1111-1111-111111111302"), "session", "Готов(а) к сессионной работе"),
                new CollaborationGoal(new Guid("11111111-1111-1111-1111-111111111303"), "collaboration", "Открыт(а) к совместным проектам"),
                new CollaborationGoal(new Guid("11111111-1111-1111-1111-111111111304"), "producer", "Ищу продюсера"),
                new CollaborationGoal(new Guid("11111111-1111-1111-1111-111111111305"), "artist", "Ищу исполнителя для песен")
            );
        }
    }
}