using System;
using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Validators.Events;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Events
{
    public class UpdateEventCommandValidatorTests : TestBase
    {
        private readonly UpdateEventCommandValidator _validator;

        public UpdateEventCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new UpdateEventCommandValidator();
        }

        [Fact]
        public void Validate_EmptyEventId_HasError()
        {
            var command = new UpdateEventCommand();
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EventId);
        }

        [Fact]
        public void Validate_LongTitle_HasError()
        {
            var command = new UpdateEventCommand
            {
                EventId = Guid.NewGuid(),
                Title = new string('A', 201)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_PastStartDate_HasError()
        {
            var command = new UpdateEventCommand
            {
                EventId = Guid.NewGuid(),
                StartDateTime = DateTime.UtcNow.AddDays(-1)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StartDateTime);
        }

        [Fact]
        public void Validate_NegativeMaxParticipants_HasError()
        {
            var command = new UpdateEventCommand
            {
                EventId = Guid.NewGuid(),
                MaxParticipants = -1
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaxParticipants);
        }

        [Fact]
        public void Validate_EndBeforeStart_HasError()
        {
            var command = new UpdateEventCommand
            {
                EventId = Guid.NewGuid(),
                StartDateTime = DateTime.UtcNow.AddDays(5),
                EndDateTime = DateTime.UtcNow.AddDays(4)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EndDateTime);
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new UpdateEventCommand
            {
                EventId = Guid.NewGuid(),
                Title = "Updated Title",
                Address = "New Address",
                StartDateTime = DateTime.UtcNow.AddDays(10),
                MaxParticipants = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}