using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Auth
{
    public class LoginCommandHandlerTests : TestBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _userRepository = Substitute.For<IUserRepository>();
            _configuration = Substitute.For<IConfiguration>();
            _verificationCodeService = Substitute.For<IVerificationCodeService>();
            _handler = new LoginCommandHandler(_userRepository, _configuration, _verificationCodeService);
            SetupJwtConfig();
        }

        private void SetupJwtConfig()
        {
            _configuration["Jwt:Key"].Returns("SuperSecretTestKeyForTestingOnly123!");
            _configuration["Jwt:Issuer"].Returns("MusicianFinder");
            _configuration["Jwt:Audience"].Returns("MusicianFinder");
        }

        [Fact]
        public async Task Handle_ValidCodeNewUser_CreatesUserAndReturnsToken()
        {
            var command = new LoginCommand { Email = "new@example.com", Code = "123456" };
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(true);
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

            var response = await _handler.Handle(command, CancellationToken.None);
            response.Success.Should().BeTrue();
            response.Token.Should().NotBeNullOrEmpty();
            response.User.Email.Should().Be(command.Email);
            response.User.ProfileCreated.Should().BeFalse();
            _userRepository.Received(1).Add(Arg.Is<User>(u => u.Email == command.Email));
        }

        [Fact]
        public async Task Handle_ValidCodeExistingUser_ReturnsToken()
        {
            var command = new LoginCommand { Email = "existing@example.com", Code = "123456" };
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(true);
            var existingUser = new User(command.Email);
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(existingUser);

            var response = await _handler.Handle(command, CancellationToken.None);
            response.Success.Should().BeTrue();
            response.User.Email.Should().Be(command.Email);
            _userRepository.DidNotReceive().Add(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_InvalidCode_ThrowsValidationException()
        {
            var command = new LoginCommand { Email = "user@example.com", Code = "wrong" };
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("*Неверный*код*");
        }

        [Fact]
        public async Task Handle_ExpiredCode_ThrowsValidationException()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "expired" };
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("*Неверный*код*");
        }

        [Fact]
        public async Task Handle_AlreadyUsedCode_ThrowsValidationException()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "used" };
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("*Неверный*код*");
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            var command = new LoginCommand { Email = "test@example.com", Code = "123456" };
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _verificationCodeService.ValidateCodeAsync(command.Email, command.Code, Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<bool>(x.Arg<CancellationToken>()));

            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}