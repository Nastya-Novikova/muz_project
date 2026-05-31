using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class VkUserIdTests : TestBase
    {
        public VkUserIdTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("123456789")]
        public void Constructor_ValidId_CreatesInstance(string id)
        {
            var vkId = new VkUserId(id);
            vkId.Value.Should().Be(id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("-123")]
        [InlineData("abc")]
        public void Constructor_InvalidId_ThrowsDomainException(string invalidId)
        {
            Action act = () => new VkUserId(invalidId);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var v1 = new VkUserId("123456789");
            var v2 = new VkUserId("123456789");
            v1.Should().Be(v2);
        }
    }
}