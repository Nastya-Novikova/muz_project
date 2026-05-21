using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class TelegramHandleTests : TestBase
    {
        public TelegramHandleTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("username", "username")]
        [InlineData("@username", "username")]
        public void Constructor_ValidHandle_CreatesInstance(string input, string expected)
        {
            var handle = new TelegramHandle(input);
            handle.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("a_very_long_username_more_than_32_chars_xxxxx")]
        [InlineData("user@name")]
        public void Constructor_InvalidHandle_ThrowsDomainException(string invalidHandle)
        {
            Action act = () => new TelegramHandle(invalidHandle);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var t1 = new TelegramHandle("@username");
            var t2 = new TelegramHandle("username");
            t1.Should().Be(t2);
        }
    }
}