using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateEventCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public UpdateEventCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _dbContext.Events
                .Include(nameof(Event.Registrations))
                .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            eventEntity.Update(
                request.Title ?? eventEntity.Title,
                request.Description ?? eventEntity.Description,
                request.RegionId ?? eventEntity.RegionId,
                request.CityId ?? eventEntity.CityId,
                request.Address ?? eventEntity.Address,
                request.StartDateTime ?? eventEntity.StartDateTime,
                request.EndDateTime ?? eventEntity.EndDateTime,
                request.MaxParticipants ?? eventEntity.MaxParticipants,
                profile.Id);

            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}