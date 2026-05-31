using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MusicianFinder.Application.Behaviors;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Behaviors
{
    public class ValidationBehaviorTests : TestBase
    {
        public ValidationBehaviorTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Handle_NoValidators_CallsNext()
        {
            var behavior = new ValidationBehavior<TestRequest, string>(new List<IValidator<TestRequest>>());
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            next().Returns(Task.FromResult("result"));
            var result = await behavior.Handle(new TestRequest(), next, CancellationToken.None);
            result.Should().Be("result");
            await next.Received(1)();
        }

        [Fact]
        public async Task Handle_ValidationSucceeds_CallsNext()
        {
            var validator = Substitute.For<IValidator<TestRequest>>();
            validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());
            var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            next().Returns(Task.FromResult("result"));
            var result = await behavior.Handle(new TestRequest(), next, CancellationToken.None);
            result.Should().Be("result");
            await next.Received(1)();
        }

        [Fact]
        public async Task Handle_ValidationFails_ThrowsValidationException()
        {
            var validator = Substitute.For<IValidator<TestRequest>>();
            var failures = new List<ValidationFailure> { new ValidationFailure("Prop", "Error") };
            validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult(failures));
            var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            Func<Task> act = async () => await behavior.Handle(new TestRequest(), next, CancellationToken.None);
            await act.Should().ThrowAsync<MusicianFinder.Application.Core.Exceptions.ValidationException>();
            await next.DidNotReceive()();
        }

        public class TestRequest : IRequest<string> { }
    }
}