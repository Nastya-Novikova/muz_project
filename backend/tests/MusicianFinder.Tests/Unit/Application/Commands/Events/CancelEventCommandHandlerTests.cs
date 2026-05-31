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
    public class CancelEventCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly CancelEventCommandHandler _handler;

        public CancelEventCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new CancelEventCommandHandler(_eventRepository, _profileProvider);
        }

        [Fact]
        public async Task Handle_ValidCommand_CancelsEvent()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);

            var command = new CancelEventCommand { EventId = ev.Id };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            ev.Status.Should().Be(EventStatus.Cancelled);
        }

        [Fact]
        public async Task Handle_NotCreator_ThrowsForbiddenException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var other = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(other);

            var command = new CancelEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsNotFoundException()
        {
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Event?)null);
            var command = new CancelEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_AlreadyCancelled_ThrowsDomainException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            ev.Cancel(creator.Id);
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);

            var command = new CancelEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<Event?>(x.Arg<CancellationToken>()));

            var command = new CancelEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}