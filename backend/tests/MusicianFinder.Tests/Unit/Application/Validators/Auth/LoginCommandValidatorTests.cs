using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.Validators.Auth;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Auth
{
    public class LoginCommandValidatorTests : TestBase
    {
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "123456" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_EmptyEmail_HasError()
        {
            var command = new LoginCommand { Email = "", Code = "123456" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_InvalidEmail_HasError()
        {
            var command = new LoginCommand { Email = "not-email", Code = "123456" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_EmptyCode_HasError()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Validate_CodeNot6Digits_HasError()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "12345" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }
    }
}