using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="SendSuggestionCommand"/>.
    /// </summary>
    public class SendSuggestionCommandHandler : IRequestHandler<SendSuggestionCommand, Guid>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="profileProvider">Сервис текущего пользователя.</param>
        public SendSuggestionCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentProfileProvider profileProvider)
        {
            _profileRepository = profileRepository;
            _suggestionRepository = suggestionRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(SendSuggestionCommand request, CancellationToken cancellationToken)
        {
            var fromProfile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            var ToProfile = await _profileRepository.GetByIdWithNotificationsAsync(request.ToProfileId, cancellationToken)
                ?? throw new NotFoundException("Профиль отправителя не найден.");

            var suggestion = new CollaborationSuggestion(fromProfile.Id, request.ToProfileId, request.Message);
            _suggestionRepository.Add(suggestion);

            return suggestion.Id;
        }
    }
}