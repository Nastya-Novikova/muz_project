using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Suggestions
{
    /// <summary>
    /// Обработчик команды <see cref="SendSuggestionCommand"/>.
    /// </summary>
    public class SendSuggestionCommandHandler : IRequestHandler<SendSuggestionCommand, Guid>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public SendSuggestionCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _suggestionRepository = suggestionRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(SendSuggestionCommand request, CancellationToken cancellationToken)
        {
            var fromProfile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль отправителя не найден.");

            var suggestion = new CollaborationSuggestion(fromProfile.Id, request.ToProfileId, request.Message);
            _suggestionRepository.Add(suggestion);
            return suggestion.Id;
        }
    }
}