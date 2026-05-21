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
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Suggestions
{
    public class UpdateSuggestionStatusCommandHandlerTests : TestBase
    {
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly UpdateSuggestionStatusCommandHandler _handler;

        public UpdateSuggestionStatusCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _suggestionRepository = Substitute.For<ICollaborationSuggestionRepository>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _handler = new UpdateSuggestionStatusCommandHandler(_suggestionRepository, _profileProvider, _profileRepository);
        }

        [Fact]
        public async Task Handle_AcceptByReceiver_AcceptsSuggestion()
        {
            var receiver = new MusicianProfileBuilder().Build();
            var suggestion = new CollaborationSuggestion(Guid.NewGuid(), receiver.Id, "Test");
            _suggestionRepository.GetByIdAsync(suggestion.Id, Arg.Any<CancellationToken>()).Returns(suggestion);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(receiver);

            var command = new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, Status = SuggestionStatus.Accepted };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            suggestion.Status.Should().Be(SuggestionStatus.Accepted);
        }

        [Fact]
        public async Task Handle_AcceptBySender_ThrowsForbiddenException()
        {
            var sender = new MusicianProfileBuilder().Build();
            var receiver = new MusicianProfileBuilder().Build();
            var suggestion = new CollaborationSuggestion(sender.Id, receiver.Id, "Test");
            _suggestionRepository.GetByIdAsync(suggestion.Id, Arg.Any<CancellationToken>()).Returns(suggestion);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(sender);

            var command = new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, Status = SuggestionStatus.Accepted };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_RejectByReceiver_SetsRejected()
        {
            var receiver = new MusicianProfileBuilder().Build();
            var suggestion = new CollaborationSuggestion(Guid.NewGuid(), receiver.Id, "Test");
            _suggestionRepository.GetByIdAsync(suggestion.Id, Arg.Any<CancellationToken>()).Returns(suggestion);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(receiver);

            var command = new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, Status = SuggestionStatus.Rejected };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            suggestion.Status.Should().Be(SuggestionStatus.Rejected);
        }

        [Fact]
        public async Task Handle_SuggestionNotFound_ThrowsNotFoundException()
        {
            _suggestionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CollaborationSuggestion?)null);
            var command = new UpdateSuggestionStatusCommand { SuggestionId = Guid.NewGuid(), Status = SuggestionStatus.Accepted };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_InvalidStatus_ThrowsValidationException()
        {
            var receiver = new MusicianProfileBuilder().Build();
            var suggestion = new CollaborationSuggestion(Guid.NewGuid(), receiver.Id, "Test");
            _suggestionRepository.GetByIdAsync(suggestion.Id, Arg.Any<CancellationToken>()).Returns(suggestion);
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(receiver);

            var command = new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, Status = (SuggestionStatus)999 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _suggestionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<CollaborationSuggestion?>(x.Arg<CancellationToken>()));

            var command = new UpdateSuggestionStatusCommand { SuggestionId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}