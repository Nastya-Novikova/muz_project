using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Media
{
    public class UploadMediaCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;
        private readonly UploadMediaCommandHandler _handler;

        public UploadMediaCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _fileStorage = Substitute.For<IFileStorage>();
            _handler = new UploadMediaCommandHandler(_profileRepository, _profileProvider, _fileStorage);
        }

        [Fact]
        public async Task Handle_ValidAudio_UploadsAndReturnsMediaId()
        {
            var userId = Guid.NewGuid();
            var profile = new MusicianProfileBuilder().WithUserId(userId).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _fileStorage.SaveFileAsync(Arg.Any<Stream>(), "track.mp3", "audio/mpeg")
                .Returns("http://storage/track.mp3");

            var command = new UploadMediaCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "track.mp3",
                ContentType = "audio/mpeg",
                Title = "My Track",
                Type = MediaType.Audio
            };

            _profileRepository.ExecuteAndTrackNewOwnedAsync<PortfolioItem>(
                userId, Arg.Any<Func<MusicianProfile, PortfolioItem>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    var func = callInfo.ArgAt<Func<MusicianProfile, PortfolioItem>>(1);
                    var item = func(profile);
                    profile.AddPortfolioItem(item);
                });

            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().NotBeEmpty();
            await _fileStorage.Received(1).SaveFileAsync(Arg.Any<Stream>(), command.FileName, command.ContentType);
        }
    }
}