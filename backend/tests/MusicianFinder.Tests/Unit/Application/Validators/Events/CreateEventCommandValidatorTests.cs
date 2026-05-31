using System;
using FluentValidation.TestHelper;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Validators.Events;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Validators.Events
{
    public class CreateEventCommandValidatorTests : TestBase
    {
        private readonly CreateEventCommandValidator _validator;

        public CreateEventCommandValidatorTests(ITestOutputHelper output) : base(output)
        {
            _validator = new CreateEventCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_NoErrors()
        {
            var command = new CreateEventCommand
            {
                Title = "Test",
                RegionId = 1,
                CityId = 1,
                Address = "Addr",
                StartDateTime = DateTime.UtcNow.AddDays(7),
                MaxParticipants = 10
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_EmptyTitle_HasError()
        {
            var command = new CreateEventCommand { Title = null! };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_PastStartDate_HasError()
        {
            var command = new CreateEventCommand
            {
                Title = "Test",
                RegionId = 1,
                CityId = 1,
                Address = "Addr",
                StartDateTime = DateTime.UtcNow.AddDays(-1)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StartDateTime);
        }

        [Fact]
        public void Validate_NegativeMaxParticipants_HasError()
        {
            var command = new CreateEventCommand { MaxParticipants = -1 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaxParticipants);
        }
    }
}