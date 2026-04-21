using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Collaborations.CheckCollaboration
{
    /// <summary>
    /// Обработчик запроса <see cref="CheckCollaborationQuery"/>.
    /// </summary>
    public class CheckCollaborationQueryHandler : IRequestHandler<CheckCollaborationQuery, bool>
    {
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CheckCollaborationQueryHandler"/>.
        /// </summary>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CheckCollaborationQueryHandler(
            ICollaborationSuggestionRepository suggestionRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _suggestionRepository = suggestionRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<bool> Handle(CheckCollaborationQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var sent = await _suggestionRepository.GetSentAsync(profile.Id, 1, 1);
            return sent.Any(s => s.ToProfileId == request.CollaboratedProfileId);
        }
    }
}