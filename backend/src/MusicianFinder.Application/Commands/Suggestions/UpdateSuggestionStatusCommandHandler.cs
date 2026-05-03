using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Enums;
using FluentValidation.Results;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateSuggestionStatusCommand"/>.
    /// </summary>
    public class UpdateSuggestionStatusCommandHandler : IRequestHandler<UpdateSuggestionStatusCommand, Unit>
    {
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="profileprovider">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        public UpdateSuggestionStatusCommandHandler(
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentProfileProvider profileprovider,
            IMusicianProfileRepository profileRepository)
        {
            _suggestionRepository = suggestionRepository;
            _profileProvider = profileprovider;
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateSuggestionStatusCommand request, CancellationToken cancellationToken)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(request.SuggestionId, cancellationToken)
                ?? throw new NotFoundException("Предложение не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

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
                    throw new ValidationException(new[] { new ValidationFailure("Status", "Недопустимый статус.") });
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
                    throw new ValidationException(
                        new[] { new ValidationFailure("Status", "Недопустимый статус.") });
            }

            return Unit.Value;
        }
    }
}