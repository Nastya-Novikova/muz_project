using System;
using FluentAssertions;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.Aggregates
{
    public class UserTests : TestBase
    {
        public UserTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Constructor_ValidEmail_CreatesUser()
        {
            var user = new User("test@example.com");
            user.Email.Should().Be("test@example.com");
            user.ProfileCreated.Should().BeFalse();
            user.Role.Should().Be(UserRole.User);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_InvalidEmail_ThrowsDomainException(string invalidEmail)
        {
            Action act = () => new User(invalidEmail);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void MarkProfileAsCreated_SetsFlag()
        {
            var user = new User("test@example.com");
            user.MarkProfileAsCreated();
            user.ProfileCreated.Should().BeTrue();
        }

        [Fact]
        public void ClearMusicianProfile_ResetsFlag()
        {
            var user = new User("test@example.com");
            user.MarkProfileAsCreated();
            user.ClearMusicianProfile();
            user.ProfileCreated.Should().BeFalse();
        }

        [Fact]
        public void MarkAsDeleted_SetsIsDeleted()
        {
            var user = new User("test@example.com");
            user.MarkAsDeleted();
            user.IsDeleted.Should().BeTrue();
            user.DeletedAt.Should().NotBeNull();
        }
    }
}