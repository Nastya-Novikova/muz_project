using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Favorites;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Favorites
{
    public class RemoveFavoriteCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly RemoveFavoriteCommandHandler _handler;

        public RemoveFavoriteCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new RemoveFavoriteCommandHandler(_profileProvider);
        }

        [Fact]
        public async Task Handle_ValidRemoval_ReturnsUnit()
        {
            var profile = new MusicianProfileBuilder().Build();
            var targetId = Guid.NewGuid();
            profile.AddToFavorites(targetId);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new RemoveFavoriteCommand { TargetProfileId = targetId };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            profile.Favorites.Should().NotContain(f => f.TargetProfileId == targetId);
        }

        [Fact]
        public async Task Handle_NotFavorite_ThrowsDomainException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new RemoveFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*не найден в избранном*");
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<MusicianProfile?>(x.Arg<CancellationToken>()));

            var command = new RemoveFavoriteCommand { TargetProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}