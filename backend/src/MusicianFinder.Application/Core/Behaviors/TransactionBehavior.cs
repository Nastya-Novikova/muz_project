using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Core.Behaviors
{
    /// <summary>
    /// Поведение MediatR, оборачивающее выполнение команды в транзакцию базы данных.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IReadDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TransactionBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public TransactionBehavior(IReadDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var strategy = ((DbContext)_dbContext).Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next();
                    await transaction.CommitAsync(cancellationToken);
                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}