using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class EventTitleTests : TestBase
    {
        public EventTitleTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Constructor_ValidTitle_CreatesInstance()
        {
            var title = new EventTitle("Jazz Night");
            title.Value.Should().Be("Jazz Night");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_InvalidTitle_ThrowsDomainException(string invalidTitle)
        {
            Action act = () => new EventTitle(invalidTitle);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_LongTitle_ThrowsDomainException()
        {
            var longTitle = new string('A', 201);
            Action act = () => new EventTitle(longTitle);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var title1 = new EventTitle("Jazz");
            var title2 = new EventTitle("Jazz");
            title1.Should().Be(title2);
        }
    }
}