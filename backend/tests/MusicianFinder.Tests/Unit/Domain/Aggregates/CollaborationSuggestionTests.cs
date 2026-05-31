using System;
using FluentAssertions;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.Aggregates
{
    public class CollaborationSuggestionTests : TestBase
    {
        public CollaborationSuggestionTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Constructor_SetsPendingAndRaisesEvent()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Status.Should().Be(SuggestionStatus.Pending);
            suggestion.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "CollaborationSuggestionSent");
        }

        [Fact]
        public void Accept_WhenPending_SetsAcceptedAndRaisesEvent()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Accept();
            suggestion.Status.Should().Be(SuggestionStatus.Accepted);
            suggestion.DomainEvents.Should().Contain(e => e.GetType().Name == "CollaborationSuggestionAccepted");
        }

        [Fact]
        public void Accept_WhenNotPending_ThrowsDomainException()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Accept();
            Action act = () => suggestion.Accept();
            act.Should().Throw<DomainException>().WithMessage("*только ожидающее*");
        }

        [Fact]
        public void Reject_WhenPending_SetsRejectedAndRaisesEvent()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Reject();
            suggestion.Status.Should().Be(SuggestionStatus.Rejected);
            suggestion.DomainEvents.Should().Contain(e => e.GetType().Name == "CollaborationSuggestionRejected");
        }

        [Fact]
        public void Withdraw_WhenPending_SetsWithdrawn()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Withdraw();
            suggestion.Status.Should().Be(SuggestionStatus.Withdrawn);
        }

        [Fact]
        public void Reject_WhenNotPending_Throws()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Accept();
            Action act = () => suggestion.Reject();
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Withdraw_WhenNotPending_Throws()
        {
            var suggestion = new CollaborationSuggestionBuilder().Build();
            suggestion.Accept();
            Action act = () => suggestion.Withdraw();
            act.Should().Throw<DomainException>();
        }
    }
}