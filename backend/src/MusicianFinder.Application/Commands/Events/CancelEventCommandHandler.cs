using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="CancelEventCommand"/>.
    /// </summary>
    public class CancelEventCommandHandler : IRequestHandler<CancelEventCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CancelEventCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CancelEventCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(CancelEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.EventId);

            eventEntity.Cancel(_currentUserService.UserId);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}