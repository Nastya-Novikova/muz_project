using System;
using FluentAssertions;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.DomainEvents
{
    public class DomainEventTests : TestBase
    {
        public DomainEventTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void CollaborationSuggestionSent_SetsIds()
        {
            var suggestionId = Guid.NewGuid();
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var ev = new CollaborationSuggestionSent(suggestionId, fromId, toId);
            ev.SuggestionId.Should().Be(suggestionId);
            ev.FromProfileId.Should().Be(fromId);
            ev.ToProfileId.Should().Be(toId);
        }

        [Fact]
        public void CollaborationSuggestionAccepted_SetsSuggestionId()
        {
            var id = Guid.NewGuid();
            var ev = new CollaborationSuggestionAccepted(id);
            ev.SuggestionId.Should().Be(id);
        }

        [Fact]
        public void EventCreated_SetsEventId()
        {
            var id = Guid.NewGuid();
            var ev = new EventCreated(id);
            ev.EventId.Should().Be(id);
        }

        [Fact]
        public void EventCancelled_SetsEventId()
        {
            var id = Guid.NewGuid();
            var ev = new EventCancelled(id);
            ev.EventId.Should().Be(id);
        }

        [Fact]
        public void FavoriteAdded_SetsIds()
        {
            var addedBy = Guid.NewGuid();
            var target = Guid.NewGuid();
            var ev = new FavoriteAdded(addedBy, target);
            ev.AddedByProfileId.Should().Be(addedBy);
            ev.TargetProfileId.Should().Be(target);
        }

        [Fact]
        public void ProfileCreated_SetsIds()
        {
            var profileId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ev = new ProfileCreated(profileId, userId);
            ev.ProfileId.Should().Be(profileId);
            ev.UserId.Should().Be(userId);
        }

        [Fact]
        public void UserRegisteredToEvent_SetsIds()
        {
            var eventId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var ev = new UserRegisteredToEvent(eventId, profileId);
            ev.EventId.Should().Be(eventId);
            ev.ProfileId.Should().Be(profileId);
        }
    }
}