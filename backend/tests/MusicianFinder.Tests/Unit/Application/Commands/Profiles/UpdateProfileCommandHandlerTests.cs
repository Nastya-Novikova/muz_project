using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Profiles
{
    public class UpdateProfileCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IReferenceDataValidationService _referenceDataValidation;
        private readonly UpdateProfileCommandHandler _handler;

        public UpdateProfileCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _referenceDataValidation = Substitute.For<IReferenceDataValidationService>();
            _handler = new UpdateProfileCommandHandler(_profileProvider, _referenceDataValidation);
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesProfileAndReturnsId()
        {
            var profile = new MusicianProfileBuilder().WithFullName("Old").Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.CityExistsAsync(2, Arg.Any<CancellationToken>()).Returns(true);
            _referenceDataValidation.GenreExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var command = new UpdateProfileCommand
            {
                FullName = "New Name",
                CityId = 2,
                GenreIds = new List<int> { 1 }
            };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(profile.Id);
            profile.FullName.Value.Should().Be("New Name");
            profile.CityId.Should().Be(2);
            profile.GenreIds.Should().Contain(g => g.Value == 1);
        }

        [Fact]
        public async Task Handle_InvalidCity_ThrowsValidationException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.CityExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

            var command = new UpdateProfileCommand { CityId = 999 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_UpdateOnlyCoreInfo_RaisesOnlyCoreInfoEvent()
        {
            var profile = new MusicianProfileBuilder().WithFullName("Old").WithCityId(1).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new UpdateProfileCommand { FullName = "New Name" };
            await _handler.Handle(command, CancellationToken.None);
            profile.DomainEvents.Should().Contain(e => e is ProfileCoreInfoUpdated);
        }

        [Fact]
        public async Task Handle_UpdateOnlyContacts_RaisesOnlyContactsEvent()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new UpdateProfileCommand { Phone = "+79161234567" };
            await _handler.Handle(command, CancellationToken.None);
            profile.DomainEvents.Should().Contain(e => e is ProfileContactsUpdated);
        }

        [Fact]
        public async Task Handle_UpdateGenres_RaisesGenresChangedEvent()
        {
            var profile = new MusicianProfileBuilder().AddGenre(1).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.GenreExistsAsync(2, Arg.Any<CancellationToken>()).Returns(true);

            var command = new UpdateProfileCommand { GenreIds = new List<int> { 2 } };
            await _handler.Handle(command, CancellationToken.None);
            profile.DomainEvents.Should().Contain(e => e is ProfileGenresChanged);
        }

        [Fact]
        public async Task Handle_InvalidGenreId_ThrowsValidationException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _referenceDataValidation.GenreExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

            var command = new UpdateProfileCommand { GenreIds = new List<int> { 999 } };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<MusicianProfile?>(x.Arg<CancellationToken>()));

            var command = new UpdateProfileCommand { FullName = "Test" };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}