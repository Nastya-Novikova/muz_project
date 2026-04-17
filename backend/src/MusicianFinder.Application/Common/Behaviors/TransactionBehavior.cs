using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MusicianFinder.Application.Common.Behaviors
{
    /// <summary>
    /// Поведение MediatR, оборачивающее выполнение запроса в транзакцию базы данных.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly DbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TransactionBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public TransactionBehavior(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Если уже есть активная транзакция, не начинаем новую
            if (_dbContext.Database.CurrentTransaction != null)
                return await next();

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Используем перегрузку без явного IsolationLevel для максимальной совместимости
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                var response = await next();
                await transaction.CommitAsync(cancellationToken);
                return response;
            });
        }
    }
}