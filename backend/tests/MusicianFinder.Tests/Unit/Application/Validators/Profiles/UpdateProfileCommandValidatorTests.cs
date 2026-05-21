using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Validators.Profiles;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Profiles
{
    public class UpdateProfileCommandValidatorTests : TestBase
    {
        private readonly UpdateProfileCommandValidator _validator;

        public UpdateProfileCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new UpdateProfileCommandValidator();
        }

        [Fact]
        public void Validate_LongFullName_HasError()
        {
            var command = new UpdateProfileCommand { FullName = new string('A', 101) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Validate_NegativeExperience_HasError()
        {
            var command = new UpdateProfileCommand { Experience = -1 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Experience);
        }

        [Fact]
        public void Validate_LongPhone_HasError()
        {
            var command = new UpdateProfileCommand { Phone = "+12345678901234567890" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Validate_LongTelegram_HasError()
        {
            var command = new UpdateProfileCommand { Telegram = new string('a', 51) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Telegram);
        }

        [Fact]
        public void Validate_InvalidProfileType_HasError()
        {
            var command = new UpdateProfileCommand { ProfileType = (ProfileType)999 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProfileType);
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new UpdateProfileCommand
            {
                FullName = "New Name",
                CityId = 2,
                Experience = 10,
                Phone = "+79161234567",
                Telegram = "@musician",
                ProfileType = ProfileType.Band
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}