using System;
using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Application.Validators.Suggestions;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Suggestions
{
    public class SendSuggestionCommandValidatorTests : TestBase
    {
        private readonly SendSuggestionCommandValidator _validator;

        public SendSuggestionCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new SendSuggestionCommandValidator();
        }

        [Fact]
        public void Validate_EmptyToProfileId_HasError()
        {
            var command = new SendSuggestionCommand();
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ToProfileId);
        }

        [Fact]
        public void Validate_LongMessage_HasError()
        {
            var command = new SendSuggestionCommand
            {
                ToProfileId = Guid.NewGuid(),
                Message = new string('A', 501)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Message);
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new SendSuggestionCommand
            {
                ToProfileId = Guid.NewGuid(),
                Message = "Let's collaborate!"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}