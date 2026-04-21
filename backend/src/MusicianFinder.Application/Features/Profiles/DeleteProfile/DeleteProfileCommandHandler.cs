using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Profiles.DeleteProfile
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteProfileCommand"/>.
    /// </summary>
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeleteProfileCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public DeleteProfileCommandHandler(
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user == null)
                throw new NotFoundException(nameof(User), _currentUserService.UserId);

            if (user.MusicianProfile == null)
                throw new NotFoundException("Профиль не найден.");

            var profileId = user.MusicianProfile.Id;

            await _profileRepository.SoftDeleteAsync(profileId);

            user.ClearMusicianProfile();

            await _userRepository.UpdateAsync(user);
            return Unit.Value;
        }
    }
}