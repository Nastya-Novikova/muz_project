using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Validators.Profiles;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Profiles
{
    public class UpdateAvatarCommandValidatorTests : TestBase
    {
        private readonly UpdateAvatarCommandValidator _validator;

        public UpdateAvatarCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new UpdateAvatarCommandValidator();
        }

        [Fact]
        public void Validate_EmptyContent_HasError()
        {
            var command = new UpdateAvatarCommand
            {
                FileName = "avatar.jpg",
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_InvalidContentType_HasError()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "avatar.jpg",
                ContentType = "text/plain"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ContentType);
        }

        [Fact]
        public void Validate_EmptyFileName_HasError()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Validate_FileTooLarge_HasError()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[5 * 1024 * 1024 + 1],
                FileName = "big.jpg",
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_ValidJpeg_NoErrors()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "avatar.jpg",
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ValidPng_NoErrors()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "avatar.png",
                ContentType = "image/png"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ValidGif_NoErrors()
        {
            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "avatar.gif",
                ContentType = "image/gif"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}