using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateEventCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CreateEventCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            var eventEntity = new Event(
                request.Title,
                request.RegionId,
                request.CityId,
                request.Address,
                request.StartDateTime,
                profile.Id,
                request.Description,
                request.EndDateTime,
                request.MaxParticipants);

            await ((DbContext)_dbContext).AddAsync(eventEntity, cancellationToken);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return eventEntity.Id;
        }
    }
}