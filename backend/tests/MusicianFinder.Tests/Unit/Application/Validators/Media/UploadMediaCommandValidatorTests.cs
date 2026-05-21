using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Validators.Media;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Media
{
    public class UploadMediaCommandValidatorTests : TestBase
    {
        private readonly UploadMediaCommandValidator _validator;

        public UploadMediaCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new UploadMediaCommandValidator();
        }

        [Fact]
        public void Validate_EmptyContent_HasError()
        {
            var command = new UploadMediaCommand { Type = MediaType.Audio, FileName = "song.mp3", Title = "Song", ContentType = "audio/mpeg" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_EmptyFileName_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                Type = MediaType.Audio,
                Title = "Song",
                ContentType = "audio/mpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Validate_EmptyTitle_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "song.mp3",
                Type = MediaType.Audio,
                ContentType = "audio/mpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_EmptyContentType_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "song.mp3",
                Title = "Song",
                Type = MediaType.Audio
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ContentType);
        }

        [Fact]
        public void Validate_AudioTooLarge_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[100 * 1024 * 1024 + 1],
                FileName = "big.mp3",
                Title = "Big",
                Type = MediaType.Audio,
                ContentType = "audio/mpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_VideoTooLarge_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[500 * 1024 * 1024 + 1],
                FileName = "big.mp4",
                Title = "Big",
                Type = MediaType.Video,
                ContentType = "video/mp4"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_PhotoTooLarge_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[5 * 1024 * 1024 + 1],
                FileName = "big.jpg",
                Title = "Big",
                Type = MediaType.Photo,
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Validate_InvalidContentTypeForAudio_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1 },
                FileName = "song.mp3",
                Title = "Song",
                Type = MediaType.Audio,
                ContentType = "video/mp4"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ContentType);
        }

        [Fact]
        public void Validate_InvalidExtensionForVideo_HasError()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1 },
                FileName = "movie.mp3",
                Title = "Movie",
                Type = MediaType.Video,
                ContentType = "video/mp4"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Validate_ValidAudio_NoErrors()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "song.mp3",
                Title = "My Song",
                Type = MediaType.Audio,
                ContentType = "audio/mpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ValidVideo_NoErrors()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "clip.mp4",
                Title = "Clip",
                Type = MediaType.Video,
                ContentType = "video/mp4"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ValidPhoto_NoErrors()
        {
            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "pic.jpg",
                Title = "Photo",
                Type = MediaType.Photo,
                ContentType = "image/jpeg"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}