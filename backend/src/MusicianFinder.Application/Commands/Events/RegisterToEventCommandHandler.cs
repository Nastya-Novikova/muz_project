using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="RegisterToEventCommand"/>.
    /// </summary>
    public class RegisterToEventCommandHandler : IRequestHandler<RegisterToEventCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegisterToEventCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="notificationService">Сервис уведомлений.</param>
        public RegisterToEventCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RegisterToEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _dbContext.Events
                .Include(nameof(Event.Registrations))
                .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            eventEntity.Register(profile.Id);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationToProfileAsync(
                eventEntity.CreatorProfileId,
                NotificationType.EventRegistration,
                new Dictionary<string, object>
                {
                    ["eventId"] = eventEntity.Id,
                    ["eventTitle"] = eventEntity.Title,
                    ["participantName"] = profile.FullName
                });

            return Unit.Value;
        }
    }
}