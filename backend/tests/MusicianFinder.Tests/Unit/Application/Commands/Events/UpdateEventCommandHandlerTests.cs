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
using MusicianFinder.Domain.Enums;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Events
{
    public class UpdateEventCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IReferenceDataValidationService _referenceDataValidation;
        private readonly UpdateEventCommandHandler _handler;

        public UpdateEventCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _referenceDataValidation = Substitute.For<IReferenceDataValidationService>();
            _handler = new UpdateEventCommandHandler(_eventRepository, _profileProvider, _referenceDataValidation);
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesEventAndReturnsId()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);
            _referenceDataValidation.RegionExistsAsync(2, Arg.Any<CancellationToken>()).Returns(true);

            var command = new UpdateEventCommand { EventId = ev.Id, Title = "New Title", RegionId = 2 };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(ev.Id);
            ev.Title.Value.Should().Be("New Title");
        }

        [Fact]
        public async Task Handle_NotCreator_ThrowsForbiddenException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var other = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(other);

            var command = new UpdateEventCommand { EventId = ev.Id, Title = "Hacked" };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_EventCancelled_ThrowsDomainException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            ev.Cancel(creator.Id);
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);

            var command = new UpdateEventCommand { EventId = ev.Id, Title = "New" };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*только запланированное*");
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsNotFoundException()
        {
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Event?)null);

            var command = new UpdateEventCommand { EventId = Guid.NewGuid(), Title = "New" };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_UpdateStartDateInPast_ThrowsDomainException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);
            _referenceDataValidation.RegionExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);

            var command = new UpdateEventCommand { EventId = ev.Id, StartDateTime = DateTime.UtcNow.AddDays(-1) };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*будущем*");
        }

        [Fact]
        public async Task Handle_UpdateWithInvalidRegion_ThrowsValidationException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);
            _referenceDataValidation.RegionExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

            var command = new UpdateEventCommand { EventId = ev.Id, RegionId = 999 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<Event?>(x.Arg<CancellationToken>()));

            var command = new UpdateEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}