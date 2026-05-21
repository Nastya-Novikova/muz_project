using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Behaviors;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Behaviors
{
    public class LoggingBehaviorTests : TestBase
    {
        private readonly ILogger<LoggingBehavior<TestRequest, string>> _logger;
        private readonly LoggingBehavior<TestRequest, string> _behavior;

        public LoggingBehaviorTests(ITestOutputHelper output) : base(output)
        {
            _logger = Substitute.For<ILogger<LoggingBehavior<TestRequest, string>>>();
            _behavior = new LoggingBehavior<TestRequest, string>(_logger);
        }

        [Fact]
        public async Task Handle_LogsStartAndEndWithDuration()
        {
            var request = new TestRequest();
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            next().Returns(Task.FromResult("result"));

            var result = await _behavior.Handle(request, next, CancellationToken.None);

            result.Should().Be("result");
            // Проверяем, что были залогированы начало и конец
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Начало обработки запроса")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Запрос") && o.ToString()!.Contains("обработан за")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        public class TestRequest : IRequest<string> { }
    }
}