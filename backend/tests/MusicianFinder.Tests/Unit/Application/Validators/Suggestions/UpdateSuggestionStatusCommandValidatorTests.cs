using System;
using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Application.Validators.Suggestions;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Suggestions
{
    public class UpdateSuggestionStatusCommandValidatorTests : TestBase
    {
        private readonly UpdateSuggestionStatusCommandValidator _validator;

        public UpdateSuggestionStatusCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new UpdateSuggestionStatusCommandValidator();
        }

        [Fact]
        public void Validate_EmptySuggestionId_HasError()
        {
            var command = new UpdateSuggestionStatusCommand();
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SuggestionId);
        }

        [Fact]
        public void Validate_InvalidStatus_HasError()
        {
            var command = new UpdateSuggestionStatusCommand
            {
                SuggestionId = Guid.NewGuid(),
                Status = (SuggestionStatus)999
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Validate_AcceptedStatus_NoErrors()
        {
            var command = new UpdateSuggestionStatusCommand
            {
                SuggestionId = Guid.NewGuid(),
                Status = SuggestionStatus.Accepted
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_RejectedStatus_NoErrors()
        {
            var command = new UpdateSuggestionStatusCommand
            {
                SuggestionId = Guid.NewGuid(),
                Status = SuggestionStatus.Rejected
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}