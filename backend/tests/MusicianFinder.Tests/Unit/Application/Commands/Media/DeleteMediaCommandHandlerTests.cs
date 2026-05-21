using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Domain.Enums;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Media
{
    public class DeleteMediaCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;
        private readonly DeleteMediaCommandHandler _handler;

        public DeleteMediaCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _fileStorage = Substitute.For<IFileStorage>();
            _handler = new DeleteMediaCommandHandler(_profileProvider, _fileStorage);
        }

        [Fact]
        public async Task Handle_ValidDeletion_RemovesItemAndFile()
        {
            var profile = new MusicianProfileBuilder().Build();
            var portfolioItem = new PortfolioItem("audio/mpeg", "http://files/track.mp3", "Test", MediaType.Audio);
            profile.AddPortfolioItem(portfolioItem);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new DeleteMediaCommand { MediaId = portfolioItem.Id };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            await _fileStorage.Received(1).DeleteFileAsync("http://files/track.mp3");
            profile.Portfolio.Should().NotContain(p => p.Id == portfolioItem.Id);
        }

        [Fact]
        public async Task Handle_MediaNotFound_ThrowsNotFoundException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new DeleteMediaCommand { MediaId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_StorageFailure_ThrowsException()
        {
            var profile = new MusicianProfileBuilder().Build();
            var portfolioItem = new PortfolioItem("audio/mpeg", "http://files/track.mp3", "Test", MediaType.Audio);
            profile.AddPortfolioItem(portfolioItem);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _fileStorage.DeleteFileAsync("http://files/track.mp3")
                .Returns(Task.FromException(new InvalidOperationException("Storage error")));

            var command = new DeleteMediaCommand { MediaId = portfolioItem.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<MusicianProfile?>(x.Arg<CancellationToken>()));

            var command = new DeleteMediaCommand { MediaId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}