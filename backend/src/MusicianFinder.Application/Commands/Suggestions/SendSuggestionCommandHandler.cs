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
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public SendSuggestionCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentUserService currentUser,
            INotificationService notificationService)
        {
            _profileRepository = profileRepository;
            _suggestionRepository = suggestionRepository;
            _currentUser = currentUser;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(SendSuggestionCommand request, CancellationToken cancellationToken)
        {
            var fromProfile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль отправителя не найден.");

            var ToProfile = await _profileRepository.GetByIdWithNotificationsAsync(request.ToProfileId, cancellationToken)
                ?? throw new NotFoundException("Профиль отправителя не найден.");

            var suggestion = new CollaborationSuggestion(fromProfile.Id, request.ToProfileId, request.Message);
            _suggestionRepository.Add(suggestion);

            await _notificationService.SendNotificationToProfileAsync(
                ToProfile,
                NotificationType.CollaborationReceived,
                new Dictionary<string, object>
                {
                    ["fromProfileName"] = fromProfile.FullName.Value,
                    ["message"] = request.Message ?? "Запрос на сотрудничество",
                    ["suggestionId"] = suggestion.Id
                });

            return suggestion.Id;
        }
    }
}