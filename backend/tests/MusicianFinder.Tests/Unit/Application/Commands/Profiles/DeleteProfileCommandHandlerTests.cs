using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Profiles
{
    public class DeleteProfileCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IUserRepository _userRepository;
        private readonly DeleteProfileCommandHandler _handler;

        public DeleteProfileCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new DeleteProfileCommandHandler(_profileProvider, _userRepository);
        }

        [Fact]
        public async Task Handle_ValidCommand_SoftDeletesProfileAndClearsUserFlag()
        {
            var user = new User("test@example.com");
            user.MarkProfileAsCreated();
            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

            var command = new DeleteProfileCommand();
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            profile.IsDeleted.Should().BeTrue();
            user.ProfileCreated.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsNotFoundException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _userRepository.GetByIdAsync(profile.UserId, Arg.Any<CancellationToken>()).Returns((User?)null);

            var command = new DeleteProfileCommand();
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_AlreadyDeleted_StillSucceeds()
        {
            var user = new User("test@example.com");
            user.MarkProfileAsCreated();
            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            profile.MarkAsDeleted();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

            var command = new DeleteProfileCommand();
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            profile.IsDeleted.Should().BeTrue();
            user.ProfileCreated.Should().BeFalse();
        }
    }
}