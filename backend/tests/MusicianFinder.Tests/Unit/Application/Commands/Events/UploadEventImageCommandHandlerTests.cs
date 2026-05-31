using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Events
{
    public class UploadEventImageCommandHandlerTests : TestBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;
        private readonly UploadEventImageCommandHandler _handler;

        public UploadEventImageCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _fileStorage = Substitute.For<IFileStorage>();
            _handler = new UploadEventImageCommandHandler(_eventRepository, _profileProvider, _fileStorage);
        }

        [Fact]
        public async Task Handle_ValidImageByCreator_ReturnsUrl()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);
            _fileStorage.SaveFileAsync(Arg.Any<Stream>(), "image.jpg", "image/jpeg")
                .Returns("http://storage/img.jpg");

            var command = new UploadEventImageCommand
            {
                EventId = ev.Id,
                Content = new byte[] { 1, 2, 3 },
                FileName = "image.jpg",
                ContentType = "image/jpeg"
            };

            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be("http://storage/img.jpg");
            ev.ImageUrl.Should().Be("http://storage/img.jpg");
        }

        [Fact]
        public async Task Handle_NotCreator_ThrowsForbiddenException()
        {
            var other = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(other);

            var command = new UploadEventImageCommand { EventId = ev.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsNotFoundException()
        {
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Event?)null);
            var command = new UploadEventImageCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_StorageFailure_ThrowsException()
        {
            var creator = new MusicianProfileBuilder().Build();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _eventRepository.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(creator);
            _fileStorage.SaveFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromException<string>(new Exception("MinIO down")));

            var command = new UploadEventImageCommand { EventId = ev.Id, Content = new byte[] { 1 } };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>().WithMessage("MinIO down");
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _eventRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<Event?>(x.Arg<CancellationToken>()));

            var command = new UploadEventImageCommand { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}