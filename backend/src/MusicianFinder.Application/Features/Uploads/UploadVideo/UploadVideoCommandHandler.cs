using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Uploads.DTOs;

namespace MusicianFinder.Application.Features.Uploads.UploadVideo
{
    /// <summary>
    /// Обработчик команды <see cref="UploadVideoCommand"/>.
    /// </summary>
    public class UploadVideoCommandHandler : IRequestHandler<UploadVideoCommand, UploadResultDto>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPortfolioVideoRepository _videoRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadVideoCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="videoRepository">Репозиторий видеозаписей.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public UploadVideoCommandHandler(
            IProfileRepository profileRepository,
            IPortfolioVideoRepository videoRepository,
            IFileStorage fileStorage,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _videoRepository = videoRepository;
            _fileStorage = fileStorage;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<UploadResultDto> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);

            var video = new PortfolioVideo(profile.Id, request.Title, fileUrl, request.ContentType, 0, request.Description);
            await _videoRepository.AddAsync(video);

            return _mapper.Map<UploadResultDto>(video);
        }
    }
}