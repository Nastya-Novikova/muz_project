using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Factories;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Events
{
    public class CreateEventCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IReferenceDataValidationService _referenceDataValidation;
        private readonly CreateEventCommandHandler _handler;

        public CreateEventCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _referenceDataValidation = Substitute.For<IReferenceDataValidationService>();
            _handler = new CreateEventCommandHandler(_eventRepository, _profileProvider, _referenceDataValidation);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesEventAndReturnsId()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.RegionExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _referenceDataValidation.CityExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateEventCommand
            {
                Title = "Test Event",
                RegionId = 1,
                CityId = 1,
                Address = "Addr",
                StartDateTime = DateTime.UtcNow.AddDays(7),
                MaxParticipants = 10
            };

            var eventId = await _handler.Handle(command, CancellationToken.None);
            eventId.Should().NotBeEmpty();
            _eventRepository.Received(1).Add(Arg.Is<Event>(e => e.Title.Value == command.Title));
        }

        [Fact]
        public async Task Handle_InvalidRegion_ThrowsValidationException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.RegionExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);
            _referenceDataValidation.CityExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateEventCommand { Title = "Test", RegionId = 999, CityId = 1, Address = "Addr", StartDateTime = DateTime.UtcNow.AddDays(7) };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_InvalidCityId_ThrowsValidationException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.RegionExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _referenceDataValidation.CityExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateEventCommand { Title = "Test", RegionId = 1, CityId = 999, Address = "Addr", StartDateTime = DateTime.UtcNow.AddDays(7) };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}