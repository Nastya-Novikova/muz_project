using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.SharedKernel;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Commands.Base;

namespace MusicianFinder.Application.Behaviors
{
    /// <summary>
    /// Поведение MediatR, оборачивающее выполнение команды в транзакцию базы данных.
    /// После выполнения команды диспатчит доменные события из отслеживаемых агрегатов и сохраняет изменения.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly DbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TransactionBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="domainEventDispatcher">Диспатчер доменных событий.</param>
        public TransactionBehavior(DbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        {
            _dbContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next();

                    // Собираем все агрегаты с событиями
                    var aggregates = _dbContext.ChangeTracker.Entries<AggregateRoot>()
                        .Select(e => e.Entity)
                        .Where(e => e.DomainEvents.Any())
                        .Distinct()
                        .ToList();

                    // Диспатчим доменные события (обработчики пишут в Outbox)
                    foreach (var entity in aggregates)
                    {
                        await _domainEventDispatcher.DispatchAsync(entity, cancellationToken);
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return response;
                }
                finally
                {
                    // Очистка событий, чтобы избежать повторного диспатча
                    var aggregates = _dbContext.ChangeTracker.Entries<AggregateRoot>()
                        .Select(e => e.Entity)
                        .Distinct();
                    foreach (var entity in aggregates)
                        entity.ClearDomainEvents();
                }
            });
        }
    }
}