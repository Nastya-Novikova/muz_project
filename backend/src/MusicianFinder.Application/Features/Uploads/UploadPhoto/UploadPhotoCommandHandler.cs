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

namespace MusicianFinder.Application.Features.Uploads.UploadPhoto
{
    /// <summary>
    /// Обработчик команды <see cref="UploadPhotoCommand"/>.
    /// </summary>
    public class UploadPhotoCommandHandler : IRequestHandler<UploadPhotoCommand, UploadResultDto>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPortfolioPhotoRepository _photoRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadPhotoCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="photoRepository">Репозиторий фотографий.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public UploadPhotoCommandHandler(
            IProfileRepository profileRepository,
            IPortfolioPhotoRepository photoRepository,
            IFileStorage fileStorage,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<UploadResultDto> Handle(UploadPhotoCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);

            var photo = new PortfolioPhoto(profile.Id, request.Title, fileUrl, request.ContentType, request.Description);
            await _photoRepository.AddAsync(photo);

            return _mapper.Map<UploadResultDto>(photo);
        }
    }
}