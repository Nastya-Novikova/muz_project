using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Auth
{
    public class RequestCodeCommandHandlerTests : TestBase
    {
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly RequestCodeCommandHandler _handler;

        public RequestCodeCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _emailService = Substitute.For<IEmailService>();
            _verificationCodeService = Substitute.For<IVerificationCodeService>();
            _handler = new RequestCodeCommandHandler(_emailService, _verificationCodeService);
        }

        [Fact]
        public async Task Handle_ValidEmail_GeneratesCodeAndSendsEmail()
        {
            var command = new RequestCodeCommand { Email = "test@example.com" };
            _verificationCodeService.GenerateAndSaveCodeAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns("123456");

            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            await _verificationCodeService.Received(1).GenerateAndSaveCodeAsync(command.Email, Arg.Any<CancellationToken>());
            await _emailService.Received(1).SendVerificationCodeAsync(command.Email, "123456");
        }

        [Fact]
        public async Task Handle_EmailServiceThrows_PropagatesException()
        {
            var command = new RequestCodeCommand { Email = "test@example.com" };
            _verificationCodeService.GenerateAndSaveCodeAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns("123456");
            _emailService.SendVerificationCodeAsync(command.Email, "123456")
                .Returns(Task.FromException(new InvalidOperationException("SMTP error")));

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("SMTP error");
        }

        [Fact]
        public async Task Handle_Cancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            _verificationCodeService.GenerateAndSaveCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromCanceled<string>(x.Arg<CancellationToken>()));
            // email можно не настраивать, т.к. до него не дойдёт

            var command = new RequestCodeCommand { Email = "test@example.com" };
            Func<Task> act = async () => await _handler.Handle(command, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}