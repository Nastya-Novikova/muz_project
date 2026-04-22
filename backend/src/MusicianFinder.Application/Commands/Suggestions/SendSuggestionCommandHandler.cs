using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="SendSuggestionCommand"/>.
    /// </summary>
    public class SendSuggestionCommandHandler : IRequestHandler<SendSuggestionCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SendSuggestionCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="notificationService">Сервис уведомлений.</param>
        public SendSuggestionCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(SendSuggestionCommand request, CancellationToken cancellationToken)
        {
            var fromProfile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Ваш профиль не найден.");

            var toProfile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == request.ToProfileId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(MusicianProfile), request.ToProfileId);

            var existing = await _dbContext.CollaborationSuggestions
                .AnyAsync(s => s.FromProfileId == fromProfile.Id && s.ToProfileId == toProfile.Id, cancellationToken);
            if (existing)
                throw new ConflictException("Предложение этому пользователю уже отправлено.");

            var suggestion = new CollaborationSuggestion(fromProfile.Id, toProfile.Id, request.Message);
            await ((DbContext)_dbContext).AddAsync(suggestion, cancellationToken);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationToProfileAsync(
                toProfile.Id,
                Domain.Enums.NotificationType.CollaborationReceived,
                new Dictionary<string, object>
                {
                    ["fromProfileName"] = fromProfile.FullName,
                    ["suggestionId"] = suggestion.Id,
                    ["message"] = suggestion.Message ?? string.Empty
                });

            return Unit.Value;
        }
    }
}