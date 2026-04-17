using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Collaborations.SendSuggestion
{
    /// <summary>
    /// Обработчик команды <see cref="SendSuggestionCommand"/>.
    /// </summary>
    public class SendSuggestionCommandHandler : IRequestHandler<SendSuggestionCommand>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SendSuggestionCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="notificationService">Сервис уведомлений.</param>
        public SendSuggestionCommandHandler(
            IProfileRepository profileRepository,
            ICollaborationSuggestionRepository suggestionRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _profileRepository = profileRepository;
            _suggestionRepository = suggestionRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task Handle(SendSuggestionCommand request, CancellationToken cancellationToken)
        {
            var fromProfile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (fromProfile == null)
                throw new NotFoundException("Ваш профиль не найден.");

            var toProfile = await _profileRepository.GetByIdAsync(request.ToProfileId);
            if (toProfile == null)
                throw new NotFoundException(nameof(MusicianProfile), request.ToProfileId);

            var suggestion = new CollaborationSuggestion(fromProfile.Id, toProfile.Id, request.Message);
            await _suggestionRepository.AddAsync(suggestion);

            await _notificationService.SendNotificationToProfileAsync(
                toProfile.Id,
                NotificationType.CollaborationReceived,
                new Dictionary<string, object>
                {
                    ["fromProfileName"] = fromProfile.FullName,
                    ["suggestionId"] = suggestion.Id,
                    ["message"] = suggestion.Message ?? string.Empty
                });
        }
    }
}