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

namespace MusicianFinder.Application.Features.Uploads.UploadAudio
{
    /// <summary>
    /// Обработчик команды <see cref="UploadAudioCommand"/>.
    /// </summary>
    public class UploadAudioCommandHandler : IRequestHandler<UploadAudioCommand, UploadResultDto>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPortfolioAudioRepository _audioRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadAudioCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="audioRepository">Репозиторий аудиозаписей.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public UploadAudioCommandHandler(
            IProfileRepository profileRepository,
            IPortfolioAudioRepository audioRepository,
            IFileStorage fileStorage,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _audioRepository = audioRepository;
            _fileStorage = fileStorage;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<UploadResultDto> Handle(UploadAudioCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);

            var audio = new PortfolioAudio(profile.Id, request.Title, fileUrl, request.ContentType, 0, request.Description);
            await _audioRepository.AddAsync(audio);

            return _mapper.Map<UploadResultDto>(audio);
        }
    }
}