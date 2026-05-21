using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Suggestions
{
    public class SendSuggestionCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly SendSuggestionCommandHandler _handler;

        public SendSuggestionCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _suggestionRepository = Substitute.For<ICollaborationSuggestionRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new SendSuggestionCommandHandler(_profileRepository, _suggestionRepository, _profileProvider);
        }

        [Fact]
        public async Task Handle_ValidCommand_SendsSuggestionAndReturnsId()
        {
            var fromProfile = new MusicianProfileBuilder().Build();
            var toProfile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(fromProfile);
            _profileRepository.GetByIdAsync(toProfile.Id, Arg.Any<CancellationToken>()).Returns(toProfile);

            var command = new SendSuggestionCommand { ToProfileId = toProfile.Id, Message = "Hi" };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().NotBeEmpty();
            _suggestionRepository.Received(1).Add(Arg.Is<CollaborationSuggestion>(s =>
                s.FromProfileId == fromProfile.Id && s.ToProfileId == toProfile.Id));
        }

        [Fact]
        public async Task Handle_ToProfileNotFound_ThrowsNotFoundException()
        {
            var fromProfile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(fromProfile);
            _profileRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((MusicianProfile?)null);

            var command = new SendSuggestionCommand { ToProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<MusicianProfile?>(x.Arg<CancellationToken>()));

            var command = new SendSuggestionCommand { ToProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}