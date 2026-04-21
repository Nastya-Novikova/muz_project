using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Uploads.DeleteAudio
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteAudioCommand"/>.
    /// </summary>
    public class DeleteAudioCommandHandler : IRequestHandler<DeleteAudioCommand, Unit>
    {
        private readonly IPortfolioAudioRepository _audioRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeleteAudioCommandHandler"/>.
        /// </summary>
        public DeleteAudioCommandHandler(
            IPortfolioAudioRepository audioRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _audioRepository = audioRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteAudioCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var audio = await _audioRepository.GetByIdAsync(request.Id);
            if (audio == null)
                throw new NotFoundException("Аудиозапись не найдена.");

            if (audio.ProfileId != profile.Id)
                throw new ForbiddenException("Нет прав на удаление этой аудиозаписи.");

            await _fileStorage.DeleteFileAsync(audio.FileUrl);

            await _audioRepository.RemoveAsync(request.Id);

            return Unit.Value;
        }
    }
}