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
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Events
{
    public class UnregisterFromEventCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly UnregisterFromEventCommandHandler _handler;

        public UnregisterFromEventCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new UnregisterFromEventCommandHandler(_eventRepository, _profileProvider);
        }

        [Fact]
        public async Task Handle_ValidCommand_UnregistersProfile()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().Build();
            ev.Register(profile.Id);
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new UnregisterFromEventCommand { EventId = ev.Id };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            ev.Registrations.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_NotRegistered_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new UnregisterFromEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsNotFoundException()
        {
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Event?)null);
            var command = new UnregisterFromEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<Event?>(x.Arg<CancellationToken>()));

            var command = new UnregisterFromEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}