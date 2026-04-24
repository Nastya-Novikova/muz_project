using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="MusicianProfile"/>.
    /// </summary>
    public class MusicianProfileConfiguration : IEntityTypeConfiguration<MusicianProfile>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MusicianProfile> builder)
        {
            builder.ToTable("MusicianProfiles");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName)
                .HasConversion(name => name.Value, value => new ProfileName(value))
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(p => p.Email).IsUnique();

            builder.Property(p => p.Age);
            builder.Property(p => p.CityId).IsRequired();

            builder.Property(p => p.Phone)
                .HasConversion(phone => phone != null ? phone.Value : null, value => value != null ? new PhoneNumber(value) : null)
                .HasMaxLength(30);

            builder.Property(p => p.Telegram)
                .HasConversion(tg => tg != null ? tg.Value : null, value => value != null ? new TelegramHandle(value) : null)
                .HasMaxLength(50);

            builder.Property(p => p.VkUserId)
                .HasConversion(vk => vk != null ? vk.Value : null, value => value != null ? new VkUserId(value) : null)
                .HasMaxLength(255);

            builder.Property(p => p.Description);
            builder.Property(p => p.AvatarUrl);
            builder.Property(p => p.Experience).HasDefaultValue(0);
            builder.Property(p => p.LookingFor).HasConversion<string>().HasMaxLength(50);
            builder.Property(p => p.NotifyByEmail).HasDefaultValue(true);
            builder.Property(p => p.NotifyByVk).HasDefaultValue(false);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.IsDeleted).IsRequired();
            builder.Property(p => p.DeletedAt);

            builder.OwnsMany(p => p.Portfolio, a =>
            {
                a.ToTable("PortfolioItems");
                a.WithOwner().HasForeignKey("ProfileId");
                a.HasKey("Id");
                a.Property(x => x.Title).HasMaxLength(100).IsRequired();
                a.Property(x => x.Description).HasMaxLength(500);
                a.Property(x => x.FileUrl).IsRequired();
                a.Property(x => x.MimeType).HasMaxLength(50).IsRequired();
                a.Property(x => x.Type).HasConversion<string>().IsRequired();
                a.Property(x => x.Duration);
                a.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.OwnsMany(p => p.Favorites, b =>
            {
                b.ToTable("Favorites");
                b.WithOwner().HasForeignKey("AddedByProfileId");
                b.HasKey("AddedByProfileId", "TargetProfileId");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.OwnsMany(p => p.Notifications, b =>
            {
                b.ToTable("Notifications");
                b.WithOwner().HasForeignKey("ProfileId");
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.Message).HasMaxLength(500);
                b.Property(x => x.Type).HasConversion<string>().HasMaxLength(50);
                b.Property(x => x.EntityType).HasConversion<string>().HasMaxLength(50);
                b.Property(x => x.IsRead).IsRequired();
                b.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Примитивные коллекции идентификаторов
            builder.OwnsMany(p => p.GenreIds, b =>
            {
                b.ToTable("MusicianProfileGenres");
                b.WithOwner().HasForeignKey("MusicianProfileId");
                b.Property<Guid>("Value").HasColumnName("GenreId");
                b.HasKey("Value");
            });

            builder.OwnsMany(p => p.SpecialtyIds, b =>
            {
                b.ToTable("MusicianProfileSpecialties");
                b.WithOwner().HasForeignKey("MusicianProfileId");
                b.Property<Guid>("Value").HasColumnName("SpecialtyId");
                b.HasKey("Value");
            });

            builder.OwnsMany(p => p.CollaborationGoalIds, b =>
            {
                b.ToTable("MusicianProfileCollaborationGoals");
                b.WithOwner().HasForeignKey("MusicianProfileId");
                b.Property<Guid>("Value").HasColumnName("CollaborationGoalId");
                b.HasKey("Value");
            });

            builder.OwnsMany(p => p.DesiredGenreIds, b =>
            {
                b.ToTable("MusicianProfileDesiredGenres");
                b.WithOwner().HasForeignKey("MusicianProfileId");
                b.Property<Guid>("Value").HasColumnName("GenreId");
                b.HasKey("Value");
            });

            builder.OwnsMany(p => p.DesiredSpecialtyIds, b =>
            {
                b.ToTable("MusicianProfileDesiredSpecialties");
                b.WithOwner().HasForeignKey("MusicianProfileId");
                b.Property<Guid>("Value").HasColumnName("SpecialtyId");
                b.HasKey("Value");
            });

            builder.Ignore(p => p.DomainEvents);
        }
    }
}