using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Uploads.DeleteVideo
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteVideoCommand"/>.
    /// </summary>
    public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, Unit>
    {
        private readonly IPortfolioVideoRepository _videoRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeleteVideoCommandHandler"/>.
        /// </summary>
        public DeleteVideoCommandHandler(
            IPortfolioVideoRepository videoRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _videoRepository = videoRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var audio = await _videoRepository.GetByIdAsync(request.Id);
            if (audio == null)
                throw new NotFoundException("Видеозапись не найдена.");

            if (audio.ProfileId != profile.Id)
                throw new ForbiddenException("Нет прав на удаление этой видеозаписи.");

            await _fileStorage.DeleteFileAsync(audio.FileUrl);

            await _videoRepository.RemoveAsync(request.Id);

            return Unit.Value;
        }
    }
}