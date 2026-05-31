using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Validators.Profiles;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Profiles
{
    public class CreateProfileCommandValidatorTests : TestBase
    {
        private readonly CreateProfileCommandValidator _validator;

        public CreateProfileCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new CreateProfileCommandValidator();
        }

        [Fact]
        public void Validate_EmptyFullName_HasError()
        {
            var command = new CreateProfileCommand();
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Validate_EmptyCityId_HasError()
        {
            var command = new CreateProfileCommand { FullName = "John", CityId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CityId);
        }

        [Fact]
        public void Validate_NegativeExperience_HasError()
        {
            var command = new CreateProfileCommand
            {
                FullName = "John",
                CityId = 1,
                Experience = -1
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Experience);
        }

        [Fact]
        public void Validate_LongPhone_HasError()
        {
            var command = new CreateProfileCommand
            {
                FullName = "John",
                CityId = 1,
                Phone = "+12345678901234567890"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Validate_LongTelegram_HasError()
        {
            var command = new CreateProfileCommand
            {
                FullName = "John",
                CityId = 1,
                Telegram = new string('a', 51)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Telegram);
        }

        [Fact]
        public void Validate_InvalidProfileType_HasError()
        {
            var command = new CreateProfileCommand
            {
                FullName = "John",
                CityId = 1,
                ProfileType = (ProfileType)999
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProfileType);
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new CreateProfileCommand
            {
                FullName = "John Doe",
                ProfileType = ProfileType.Individual,
                CityId = 1,
                Experience = 5,
                Phone = "+79161234567",
                Telegram = "@john"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}