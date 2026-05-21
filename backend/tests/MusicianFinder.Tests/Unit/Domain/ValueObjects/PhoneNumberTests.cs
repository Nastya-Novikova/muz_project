using System;
using FluentAssertions;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Domain.ValueObjects
{
    public class PhoneNumberTests : TestBase
    {
        public PhoneNumberTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("+79161234567", "+7 (916) 123 45 67")]
        [InlineData("89161234567", "+7 (916) 123 45 67")]
        [InlineData("79161234567", "+7 (916) 123 45 67")]
        [InlineData("9161234567", "+7 (916) 123 45 67")]
        public void Constructor_ValidRussianPhone_FormatsCorrectly(string input, string expected)
        {
            var phone = new PhoneNumber(input);
            phone.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        public void Constructor_InvalidPhone_ThrowsDomainException(string invalidPhone)
        {
            Action act = () => new PhoneNumber(invalidPhone);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Equals_SameValue_AreEqual()
        {
            var p1 = new PhoneNumber("+79161234567");
            var p2 = new PhoneNumber("89161234567");
            p1.Should().Be(p2);
        }
    }
}