using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MusicianFinder.Application.Behaviors;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Behaviors
{
    public class TransactionBehaviorTests : TestBase
    {
        private readonly TestDbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly TransactionBehavior<TestCommand, MediatR.Unit> _behavior;
        private readonly DatabaseFacade _databaseFacade;
        private readonly IDbContextTransaction _transaction;
        private readonly IExecutionStrategy _executionStrategy;

        public TransactionBehaviorTests(ITestOutputHelper output) : base(output)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = Substitute.ForPartsOf<TestDbContext>(options);
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _databaseFacade = Substitute.For<DatabaseFacade>(_dbContext);
            _transaction = Substitute.For<IDbContextTransaction>();
            _executionStrategy = new TestExecutionStrategy();

            _dbContext.Database.Returns(_databaseFacade);
            _databaseFacade.CreateExecutionStrategy().Returns(_executionStrategy);
            _databaseFacade.BeginTransactionAsync(Arg.Any<CancellationToken>())
                .Returns(_transaction);

            _behavior = new TransactionBehavior<TestCommand, MediatR.Unit>(_dbContext, _domainEventDispatcher);
        }

        [Fact]
        public async Task Handle_Success_CommitsTransaction()
        {
            var command = new TestCommand();
            var next = Substitute.For<RequestHandlerDelegate<MediatR.Unit>>();
            next().Returns(Task.FromResult(MediatR.Unit.Value));

            var result = await _behavior.Handle(command, next, CancellationToken.None);

            result.Should().Be(MediatR.Unit.Value);
            await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Exception_RollsBackTransaction()
        {
            var command = new TestCommand();
            var next = Substitute.For<RequestHandlerDelegate<MediatR.Unit>>();
            next().Returns(Task.FromException<MediatR.Unit>(new InvalidOperationException("Error")));

            Func<Task> act = async () => await _behavior.Handle(command, next, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
            // CommitAsync не должен вызываться при ошибке
            await _transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ConcurrencyException_ThrowsConflictException()
        {
            var command = new TestCommand();
            var next = Substitute.For<RequestHandlerDelegate<MediatR.Unit>>();
            next().Returns(Task.FromResult(MediatR.Unit.Value));

            _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromException<int>(new DbUpdateConcurrencyException("Concurrency", new Exception())));

            Func<Task> act = async () => await _behavior.Handle(command, next, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("*изменены другим пользователем*");
            await _transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Простейшая реализация IExecutionStrategy, которая просто выполняет переданную операцию.
        /// </summary>
        private class TestExecutionStrategy : IExecutionStrategy
        {
            public bool RetriesOnFailure => false;

            public TResult Execute<TState, TResult>(
                TState state,
                Func<DbContext, TState, TResult> operation,
                Func<DbContext, TState, ExecutionResult<TResult>> verifySucceeded)
            {
                return operation(null!, state);
            }

            public async Task<TResult> ExecuteAsync<TState, TResult>(
                TState state,
                Func<DbContext, TState, CancellationToken, Task<TResult>> operation,
                Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
                CancellationToken cancellationToken)
            {
                return await operation(null!, state, cancellationToken);
            }
        }

        /// <summary>
        /// Тестовый DbContext с поддержкой InMemory, чтобы ChangeTracker работал.
        /// </summary>
        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        }

        private class TestCommand : IBaseCommand
        {
            public string IdempotencyKey { get; set; } = string.Empty;
        }
    }
}