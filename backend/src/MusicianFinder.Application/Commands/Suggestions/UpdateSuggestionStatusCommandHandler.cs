using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateSuggestionStatusCommand"/>.
    /// </summary>
    public class UpdateSuggestionStatusCommandHandler : IRequestHandler<UpdateSuggestionStatusCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateSuggestionStatusCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public UpdateSuggestionStatusCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateSuggestionStatusCommand request, CancellationToken cancellationToken)
        {
            var suggestion = await _dbContext.CollaborationSuggestions
                .FirstOrDefaultAsync(s => s.Id == request.SuggestionId, cancellationToken)
                ?? throw new NotFoundException(nameof(CollaborationSuggestion), request.SuggestionId);

            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            if (suggestion.ToProfileId != profile.Id)
                throw new ForbiddenException("Вы не являетесь получателем этого предложения.");

            switch (request.Status)
            {
                case SuggestionStatus.Accepted:
                    suggestion.Accept();
                    break;
                case SuggestionStatus.Rejected:
                    suggestion.Reject();
                    break;
                default:
                    throw new DomainException("Недопустимый статус. Можно только принять или отклонить предложение.");
            }

            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}