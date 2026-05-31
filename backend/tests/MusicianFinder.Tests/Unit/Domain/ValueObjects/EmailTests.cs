using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class EmailTests : TestBase
    {
        public EmailTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name+tag@domain.co.uk")]
        public void Constructor_ValidEmail_CreatesInstance(string validEmail)
        {
            var email = new Email(validEmail);
            email.Value.Should().Be(validEmail.ToLowerInvariant());
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Constructor_InvalidEmail_ThrowsDomainException(string invalidEmail)
        {
            Action act = () => new Email(invalidEmail);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var email1 = new Email("test@example.com");
            var email2 = new Email("test@example.com");
            email1.Should().Be(email2);
        }

        [Fact]
        public void Equals_DifferentCase_AreEqual()
        {
            var email1 = new Email("Test@Example.com");
            var email2 = new Email("test@example.com");
            email1.Should().Be(email2);
        }
    }
}