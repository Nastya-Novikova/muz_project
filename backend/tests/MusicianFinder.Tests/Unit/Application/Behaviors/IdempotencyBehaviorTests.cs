using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Behaviors;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Behaviors
{
    public class IdempotencyBehaviorTests : TestBase
    {
        private readonly IIdempotencyStore _store;
        private readonly IdempotencyBehavior<TestCommand, string> _behavior;

        public IdempotencyBehaviorTests(ITestOutputHelper output) : base(output)
        {
            _store = Substitute.For<IIdempotencyStore>();
            _behavior = new IdempotencyBehavior<TestCommand, string>(_store);
        }

        [Fact]
        public async Task Handle_NoIdempotencyKey_CallsNext()
        {
            var command = new TestCommand { IdempotencyKey = null! };
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            next().Returns(Task.FromResult("result"));
            var result = await _behavior.Handle(command, next, CancellationToken.None);
            result.Should().Be("result");
            await next.Received(1)();
            await _store.DidNotReceive().TryCreateAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_NewKey_CreatesRecordAndCallsNext()
        {
            var command = new TestCommand { IdempotencyKey = "key1" };
            _store.TryCreateAsync("key1", Arg.Any<string>()).Returns((true, null));
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            next().Returns(Task.FromResult("response"));
            var result = await _behavior.Handle(command, next, CancellationToken.None);
            result.Should().Be("response");
            await next.Received(1)();
            await _store.Received(1).UpdateAsync("key1", Arg.Is<string>(s => s.Contains("response")), "Completed");
        }

        [Fact]
        public async Task Handle_ExistingKeyWithSameHash_ReturnsStoredResponse()
        {
            var command = new TestCommand { IdempotencyKey = "key2" };
            var requestHash = ComputeHash(command);
            var existingRecord = new IdempotencyRecord
            {
                Key = "key2",
                RequestHash = requestHash,
                Response = JsonSerializer.Serialize("stored"),
                Status = "Completed"
            };
            _store.TryCreateAsync("key2", Arg.Any<string>()).Returns((false, existingRecord));
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            var result = await _behavior.Handle(command, next, CancellationToken.None);
            result.Should().Be("stored");
            await next.DidNotReceive()();
        }

        [Fact]
        public async Task Handle_ExistingKeyWithDifferentHash_ThrowsIdempotencyConflictException()
        {
            var command = new TestCommand { IdempotencyKey = "key3" };
            var existingRecord = new IdempotencyRecord
            {
                Key = "key3",
                RequestHash = "different_hash",
                Response = null,
                Status = "Completed"
            };
            _store.TryCreateAsync("key3", Arg.Any<string>()).Returns((false, existingRecord));
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            Func<Task> act = async () => await _behavior.Handle(command, next, CancellationToken.None);
            await act.Should().ThrowAsync<IdempotencyConflictException>();
        }

        [Fact]
        public async Task Handle_ExistingKeyInProgress_ThrowsIdempotencyConflictException()
        {
            var command = new TestCommand { IdempotencyKey = "key4" };
            var requestHash = ComputeHash(command);
            var existingRecord = new IdempotencyRecord
            {
                Key = "key4",
                RequestHash = requestHash,
                Response = null,
                Status = "InProgress"
            };
            _store.TryCreateAsync("key4", Arg.Any<string>()).Returns((false, existingRecord));
            var next = Substitute.For<RequestHandlerDelegate<string>>();
            Func<Task> act = async () => await _behavior.Handle(command, next, CancellationToken.None);
            await act.Should().ThrowAsync<IdempotencyConflictException>();
        }

        private static string ComputeHash(TestCommand command)
        {
            var json = JsonSerializer.Serialize(command);
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(bytes);
        }

        private class TestCommand : IBaseCommand
        {
            public string IdempotencyKey { get; set; } = string.Empty;
        }
    }
}