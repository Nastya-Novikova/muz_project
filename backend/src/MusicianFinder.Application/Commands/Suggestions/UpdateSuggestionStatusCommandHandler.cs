using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateSuggestionStatusCommand"/>.
    /// </summary>
    public class UpdateSuggestionStatusCommandHandler : IRequestHandler<UpdateSuggestionStatusCommand, Unit>
    {
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        public UpdateSuggestionStatusCommandHandler(
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentUserService currentUser,
            IMusicianProfileRepository profileRepository)
        {
            _suggestionRepository = suggestionRepository;
            _currentUser = currentUser;
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateSuggestionStatusCommand request, CancellationToken cancellationToken)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(request.SuggestionId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Предложение не найдено.");

            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            switch (request.Status)
            {
                case SuggestionStatus.Accepted:
                case SuggestionStatus.Rejected:
                    if (suggestion.ToProfileId != profile.Id)
                        throw new ForbiddenException("Только получатель может принять или отклонить предложение.");
                    break;
                case SuggestionStatus.Withdrawn:
                    if (suggestion.FromProfileId != profile.Id)
                        throw new ForbiddenException("Только отправитель может отозвать предложение.");
                    break;
                default:
                    throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Status", "Недопустимый статус.") });
            }

            switch (request.Status)
            {
                case SuggestionStatus.Accepted:
                    suggestion.Accept();
                    break;
                case SuggestionStatus.Rejected:
                    suggestion.Reject();
                    break;
                default:
                    throw new Application.Core.Exceptions.ValidationException(
                        new[] { new FluentValidation.Results.ValidationFailure("Status", "Недопустимый статус.") });
            }

            return Unit.Value;
        }
    }
}