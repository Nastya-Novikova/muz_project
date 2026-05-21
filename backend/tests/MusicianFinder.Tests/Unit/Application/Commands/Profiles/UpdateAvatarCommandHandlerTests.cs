using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Profiles
{
    public class UpdateAvatarCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;
        private readonly UpdateAvatarCommandHandler _handler;

        public UpdateAvatarCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _fileStorage = Substitute.For<IFileStorage>();
            _handler = new UpdateAvatarCommandHandler(_profileProvider, _fileStorage);
        }

        [Fact]
        public async Task Handle_ValidImage_UpdatesAvatarUrl()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _fileStorage.SaveFileAsync(Arg.Any<Stream>(), "avatar.jpg", "image/jpeg")
                .Returns("http://storage/avatar.jpg");

            var command = new UpdateAvatarCommand
            {
                Content = new byte[] { 1, 2, 3 },
                FileName = "avatar.jpg",
                ContentType = "image/jpeg"
            };
            var url = await _handler.Handle(command, CancellationToken.None);
            url.Should().Be("http://storage/avatar.jpg");
            profile.AvatarUrl.Should().Be("http://storage/avatar.jpg");
        }

        [Fact]
        public async Task Handle_StorageFailure_ThrowsException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _fileStorage.SaveFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromException<string>(new System.Exception("MinIO error")));

            var command = new UpdateAvatarCommand { Content = new byte[] { 1 } };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<System.Exception>().WithMessage("MinIO error");
        }
    }
}