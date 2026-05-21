using System;
using System.Linq;
using FluentAssertions;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.Aggregates
{
    public class MusicianProfileTests : TestBase
    {
        public MusicianProfileTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Create_ValidData_RaisesProfileCreatedEvent()
        {
            var profile = MusicianProfile.Create(Guid.NewGuid(), new ProfileName("John"), 1, "john@test.com", ProfileType.Individual);
            profile.DomainEvents.Should().ContainSingle(e => e is ProfileCreated);
        }

        [Fact]
        public void UpdateCoreInfo_ChangesPropertiesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().WithFullName("Old").WithCityId(1).Build();
            profile.UpdateCoreInfo(new ProfileName("New"), 30, "Desc", 2);
            profile.FullName.Value.Should().Be("New");
            profile.Age.Should().Be(30);
            profile.Description.Should().Be("Desc");
            profile.CityId.Should().Be(2);
            profile.DomainEvents.Should().Contain(e => e is ProfileCoreInfoUpdated);
        }

        [Fact]
        public void UpdateContacts_ChangesContactsAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.UpdateContacts(new PhoneNumber("+79161234567"), new TelegramHandle("@newtg"));
            profile.Phone!.Value.Should().Be("+7 (916) 123 45 67");
            profile.Telegram!.Value.Should().Be("newtg");
            profile.DomainEvents.Should().Contain(e => e is ProfileContactsUpdated);
        }

        [Fact]
        public void SetGenres_ReplacesGenresAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().AddGenre(1).Build();
            profile.SetGenres(new[] { new GenreId(2), new GenreId(3) });
            profile.GenreIds.Select(g => g.Value).Should().BeEquivalentTo(new[] { 2, 3 });
            profile.DomainEvents.Should().Contain(e => e is ProfileGenresChanged);
        }

        [Fact]
        public void AddToFavorites_WhenNotFavorite_AddsAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            var targetId = Guid.NewGuid();
            profile.AddToFavorites(targetId);
            profile.Favorites.Should().Contain(f => f.TargetProfileId == targetId);
            profile.DomainEvents.Should().Contain(e => e is FavoriteAdded);
        }

        [Fact]
        public void AddToFavorites_WhenAlreadyFavorite_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var targetId = Guid.NewGuid();
            profile.AddToFavorites(targetId);
            Action act = () => profile.AddToFavorites(targetId);
            act.Should().Throw<DomainException>().WithMessage("*уже в избранном*");
        }

        [Fact]
        public void RemoveFromFavorites_WhenExists_RemovesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            var targetId = Guid.NewGuid();
            profile.AddToFavorites(targetId);
            profile.RemoveFromFavorites(targetId);
            profile.Favorites.Should().NotContain(f => f.TargetProfileId == targetId);
            profile.DomainEvents.Should().Contain(e => e is FavoriteRemoved);
        }

        [Fact]
        public void RemoveFromFavorites_WhenNotExists_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            Action act = () => profile.RemoveFromFavorites(Guid.NewGuid());
            act.Should().Throw<DomainException>().WithMessage("*не найден в избранном*");
        }

        [Fact]
        public void MarkAsDeleted_SetsIsDeletedAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.MarkAsDeleted();
            profile.IsDeleted.Should().BeTrue();
            profile.DeletedAt.Should().NotBeNull();
            profile.DomainEvents.Should().Contain(e => e is ProfileDeleted);
        }

        // Дополнения MusicianProfileTests.cs
        [Fact]
        public void SetGenres_EmptyList_ClearsGenres()
        {
            var profile = new MusicianProfileBuilder().AddGenre(1).Build();
            profile.SetGenres(new List<GenreId>());
            profile.GenreIds.Should().BeEmpty();
            profile.DomainEvents.Should().Contain(e => e is ProfileGenresChanged);
        }

        [Fact]
        public void SetSpecialties_ReplacesSpecialtiesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.SetSpecialties(new[] { new SpecialtyId(1), new SpecialtyId(2) });
            profile.SpecialtyIds.Select(s => s.Value).Should().BeEquivalentTo(new[] { 1, 2 });
            profile.DomainEvents.Should().Contain(e => e is ProfileSpecialtiesChanged);
        }

        [Fact]
        public void SetCollaborationGoals_ReplacesGoalsAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.SetCollaborationGoals(new[] { new CollaborationGoalId(1) });
            profile.CollaborationGoalIds.Should().ContainSingle(g => g.Value == 1);
            profile.DomainEvents.Should().Contain(e => e is ProfileCollaborationGoalsChanged);
        }

        [Fact]
        public void SetDesiredGenres_ReplacesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.SetDesiredGenres(new[] { new GenreId(10) });
            profile.DesiredGenreIds.Should().ContainSingle(g => g.Value == 10);
            profile.DomainEvents.Should().Contain(e => e is ProfileDesiredGenresChanged);
        }

        [Fact]
        public void SetDesiredSpecialties_ReplacesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.SetDesiredSpecialties(new[] { new SpecialtyId(20) });
            profile.DesiredSpecialtyIds.Should().ContainSingle(s => s.Value == 20);
            profile.DomainEvents.Should().Contain(e => e is ProfileDesiredSpecialtiesChanged);
        }

        [Fact]
        public void AddPortfolioItem_AddsItemAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            var item = new PortfolioItem("audio/mpeg", "url", "test", MediaType.Audio);
            var added = profile.AddPortfolioItem(item);
            added.Should().Be(item);
            profile.Portfolio.Should().Contain(item);
            profile.DomainEvents.Should().Contain(e => e is PortfolioItemAdded);
        }

        [Fact]
        public void RemovePortfolioItem_WhenExists_RemovesAndRaisesEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            var item = new PortfolioItem("audio/mpeg", "url", "test", MediaType.Audio);
            profile.AddPortfolioItem(item);
            profile.RemovePortfolioItem(item.Id);
            profile.Portfolio.Should().NotContain(item);
            profile.DomainEvents.Should().Contain(e => e is PortfolioItemRemoved);
        }

        [Fact]
        public void SetAvatar_UpdatesAvatarUrl()
        {
            var profile = new MusicianProfileBuilder().Build();
            profile.SetAvatar("http://new.avatar");
            profile.AvatarUrl.Should().Be("http://new.avatar");
        }

        [Fact]
        public void UpdateNotificationPreferences_ChangesFlags()
        {
            var profile = new MusicianProfileBuilder().WithNotifyByEmail(true).Build();
            profile.UpdateNotificationPreferences(false, true);
            profile.NotifyByEmail.Should().BeFalse();
            profile.NotifyByVk.Should().BeTrue();
        }
    }
}