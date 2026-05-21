using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Favorites;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Favorites
{
    public class AddFavoriteCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly AddFavoriteCommandHandler _handler;

        public AddFavoriteCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _handler = new AddFavoriteCommandHandler(_profileRepository, _profileProvider, _currentUserService);
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsFavorite()
        {
            var userId = Guid.NewGuid();
            _currentUserService.UserId.Returns(userId);
            _profileRepository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                userId, Arg.Any<Func<MusicianProfile, Favorite>>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            var command = new AddFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            await _profileRepository.Received(1).ExecuteAndTrackNewOwnedAsync<Favorite>(
                userId, Arg.Any<Func<MusicianProfile, Favorite>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_AlreadyFavorite_ThrowsConflictException()
        {
            var userId = Guid.NewGuid();
            _currentUserService.UserId.Returns(userId);
            _profileRepository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                userId, Arg.Any<Func<MusicianProfile, Favorite>>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new DomainException("Этот профиль уже в избранном"));

            var command = new AddFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task Handle_TargetProfileNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _currentUserService.UserId.Returns(userId);
            _profileRepository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                userId, Arg.Any<Func<MusicianProfile, Favorite>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new NotFoundException("Профиль не найден")));

            var command = new AddFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileRepository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                _currentUserService.UserId,
                Arg.Any<Func<MusicianProfile, Favorite>>(),
                Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled(x.Arg<CancellationToken>()));

            var command = new AddFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}