using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.SharedKernel;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Behaviors
{
    /// <summary>
    /// Поведение MediatR, оборачивающее выполнение команды в транзакцию базы данных.
    /// После выполнения команды диспатчит доменные события из отслеживаемых агрегатов и сохраняет изменения.
    /// Включает защиту от изменений в ChangeTracker во время обработки доменных событий.
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

                    var aggregates = _dbContext.ChangeTracker.Entries<AggregateRoot>()
                        .Select(e => e.Entity)
                        .Where(e => e.DomainEvents.Any())
                        .Distinct()
                        .ToList();

                    await DispatchDomainEventsAsync(aggregates, cancellationToken);

                    //await _dbContext.SaveChangesAsync(cancellationToken);

                    System.Diagnostics.Debug.WriteLine($"=== States at {DateTime.UtcNow:O} ===");
                    foreach (var entry in _dbContext.ChangeTracker.Entries())
                    {
                        System.Diagnostics.Debug.WriteLine($"{entry.Entity.GetType().Name} ({entry.Property("Id").CurrentValue}) -> {entry.State}");
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        throw new ConflictException("Данные были изменены другим пользователем. Попробуйте обновить страницу и повторить операцию.", ex);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return response;
                }
                finally
                {
                    ClearDomainEvents();
                }
            });
        }

        /// <summary>
        /// Диспатчит доменные события с защитой от изменений состояния ChangeTracker.
        /// </summary>
        private async Task DispatchDomainEventsAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken ct)
        {
            var snapshot = SaveChangeTrackerSnapshot();

            foreach (var entity in aggregates)
            {
                await _domainEventDispatcher.DispatchAsync(entity, ct);
            }

            /*var newSnapshot = SaveChangeTrackerSnapshot();
            if (!ChangeTrackerSnapshotsEqual(snapshot, newSnapshot))
            {
                throw new InvalidOperationException(
                    "Обработчик доменного события изменил состояние ChangeTracker. " +
                    "Обработчики не должны модифицировать агрегаты или другие сущности.");
            }*/
        }

        /// <summary>
        /// Сохраняет слепок текущего состояния ChangeTracker (сущности и их состояния).
        /// </summary>
        private Dictionary<object, EntityState> SaveChangeTrackerSnapshot()
        {
            return _dbContext.ChangeTracker.Entries()
                .Where(e => e.Entity is not IInfrastructureEntity
                            && e.State != EntityState.Unchanged
                            && e.State != EntityState.Detached)
                .ToDictionary(e => e.Entity, e => e.State, ReferenceEqualityComparer.Instance);
        }

        /// <summary>
        /// Сравнивает два слепка ChangeTracker.
        /// </summary>
        private static bool ChangeTrackerSnapshotsEqual(
            Dictionary<object, EntityState> first, Dictionary<object, EntityState> second)
        {
            if (first.Count != second.Count)
                return false;

            foreach (var kvp in first)
            {
                if (!second.TryGetValue(kvp.Key, out var state) || state != kvp.Value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Очищает доменные события во всех отслеживаемых агрегатах.
        /// </summary>
        private void ClearDomainEvents()
        {
            var aggregates = _dbContext.ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Distinct();
            foreach (var entity in aggregates)
                entity.ClearDomainEvents();
        }
    }
}