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
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Profiles
{
    public class CreateProfileCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IReferenceDataValidationService _referenceDataValidation;
        private readonly CreateProfileCommandHandler _handler;

        public CreateProfileCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _referenceDataValidation = Substitute.For<IReferenceDataValidationService>();
            _handler = new CreateProfileCommandHandler(_profileRepository, _userRepository, _currentUser, _referenceDataValidation);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesProfileAndReturnsId()
        {
            var userId = Guid.NewGuid();
            var user = new User("test@example.com");
            _currentUser.UserId.Returns(userId);
            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _referenceDataValidation.CityExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateProfileCommand
            {
                FullName = "John Doe",
                ProfileType = ProfileType.Individual,
                CityId = 1,
                Experience = 5
            };

            var profileId = await _handler.Handle(command, CancellationToken.None);
            profileId.Should().NotBeEmpty();
            _profileRepository.Received(1).Add(Arg.Any<MusicianProfile>());
            user.ProfileCreated.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_UserAlreadyHasProfile_ThrowsConflictException()
        {
            var userId = Guid.NewGuid();
            var user = new User("test@example.com");
            user.MarkProfileAsCreated();
            _currentUser.UserId.Returns(userId);
            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

            var command = new CreateProfileCommand { FullName = "John", ProfileType = ProfileType.Individual, CityId = 1 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task Handle_InvalidCityId_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var user = new User("test@example.com");
            _currentUser.UserId.Returns(userId);
            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _referenceDataValidation.CityExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateProfileCommand { FullName = "John", ProfileType = ProfileType.Individual, CityId = 999 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsNotFoundException()
        {
            _currentUser.UserId.Returns(Guid.NewGuid());
            _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

            var command = new CreateProfileCommand { FullName = "John", ProfileType = ProfileType.Individual, CityId = 1 };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<User?>(x.Arg<CancellationToken>()));

            var command = new CreateProfileCommand { FullName = "John", ProfileType = ProfileType.Individual, CityId = 1 };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}