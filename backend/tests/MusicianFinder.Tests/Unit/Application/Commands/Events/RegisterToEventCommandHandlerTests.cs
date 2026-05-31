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
    public class RegisterToEventCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly RegisterToEventCommandHandler _handler;

        public RegisterToEventCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new RegisterToEventCommandHandler(_eventRepository, _profileProvider);
        }

        [Fact]
        public async Task Handle_ValidCommand_RegistersProfile()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            var command = new RegisterToEventCommand { EventId = Guid.NewGuid() };
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                command.EventId, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            await _eventRepository.Received(1).ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                command.EventId, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsNotFoundException()
        {
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(new MusicianProfileBuilder().Build());
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                Arg.Any<Guid>(), Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new NotFoundException("Мероприятие не найдено")));

            var command = new RegisterToEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_AlreadyRegistered_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().Build();
            ev.Register(profile.Id);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    var func = callInfo.ArgAt<Func<Event, EventRegistration>>(1);
                    func(ev); // вызовет DomainException, т.к. уже зарегистрирован
                });

            var command = new RegisterToEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*уже зарегистрирован*");
        }

        [Fact]
        public async Task Handle_EventCancelled_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().Build();
            ev.Cancel(ev.CreatorProfileId);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    var func = callInfo.ArgAt<Func<Event, EventRegistration>>(1);
                    func(ev); // вызов Register на отменённом
                });

            var command = new RegisterToEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*отменённое*");
        }

        [Fact]
        public async Task Handle_EventFull_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithMaxParticipants(1).Build();
            ev.Register(Guid.NewGuid()); // лимит достигнут
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    var func = callInfo.ArgAt<Func<Event, EventRegistration>>(1);
                    func(ev); // выбросит DomainException
                });

            var command = new RegisterToEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*лимит*");
        }

        [Fact]
        public async Task Handle_CreatorRegistration_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(profile.Id).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id, Arg.Any<Func<Event, EventRegistration>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    var func = callInfo.ArgAt<Func<Event, EventRegistration>>(1);
                    func(ev);
                });

            var command = new RegisterToEventCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*Создатель*");
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<MusicianProfile?>(x.Arg<CancellationToken>()));

            var command = new RegisterToEventCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}