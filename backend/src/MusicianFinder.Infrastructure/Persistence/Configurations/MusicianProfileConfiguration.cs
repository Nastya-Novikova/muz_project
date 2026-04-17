using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

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
            builder.ToTable("MusicianProfile");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.Telegram).HasMaxLength(50);
            builder.Property(p => p.VkUserId).HasMaxLength(255);
            builder.Property(p => p.NotifyByEmail).HasDefaultValue(true);
            builder.Property(p => p.NotifyByVk).HasDefaultValue(false);
            builder.Property(p => p.Experience).HasDefaultValue(0);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.ProfileType).HasConversion<string>();
            builder.Property(p => p.LookingFor).HasConversion<string>();
            builder.Property(p => p.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(p => p.Email).IsUnique();

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Genres)
                .WithMany(g => g.Profiles)
                .UsingEntity(j => j.ToTable("ProfileGenres"));

            builder.HasMany(p => p.Specialties)
                .WithMany(s => s.Profiles)
                .UsingEntity(j => j.ToTable("ProfileSpecialties"));

            builder.HasMany(p => p.CollaborationGoals)
                .WithMany(cg => cg.Profiles)
                .UsingEntity(j => j.ToTable("ProfileCollaborationGoals"));

            builder.HasMany(p => p.DesiredGenres)
                .WithMany(g => g.ProfilesLookingForThisGenre)
                .UsingEntity(j => j.ToTable("ProfileDesiredGenres"));

            builder.HasMany(p => p.DesiredSpecialties)
                .WithMany(s => s.ProfilesLookingForThisSpecialty)
                .UsingEntity(j => j.ToTable("ProfileDesiredSpecialties"));

            builder.HasMany(p => p.AudioFiles)
                .WithOne(a => a.Profile)
                .HasForeignKey(a => a.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.VideoFiles)
                .WithOne(v => v.Profile)
                .HasForeignKey(v => v.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Photos)
                .WithOne(ph => ph.Profile)
                .HasForeignKey(ph => ph.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MusicianProfile.Genres))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.Specialties))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.CollaborationGoals))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.DesiredGenres))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.DesiredSpecialties))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.AudioFiles))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.VideoFiles))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(MusicianProfile.Photos))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}