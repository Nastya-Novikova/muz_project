using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class ProfileNameTests : TestBase
    {
        public ProfileNameTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Constructor_ValidName_CreatesInstance()
        {
            var name = new ProfileName("John Doe");
            name.Value.Should().Be("John Doe");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_InvalidName_ThrowsDomainException(string invalidName)
        {
            Action act = () => new ProfileName(invalidName);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_LongName_ThrowsDomainException()
        {
            var longName = new string('A', 101);
            Action act = () => new ProfileName(longName);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var name1 = new ProfileName("John");
            var name2 = new ProfileName("John");
            name1.Should().Be(name2);
        }
    }
}