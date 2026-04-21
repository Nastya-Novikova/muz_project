using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Domain.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicianFinder.Application.Features.Profiles.GetMedia
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMediaQuery"/>.
    /// </summary>
    public class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, MediaDto>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMediaQueryHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMediaQueryHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<MediaDto> Handle(GetMediaQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.ProfileId);
            if (profile == null)
                throw new NotFoundException(nameof(Profile), request.ProfileId);

            return new MediaDto
            {
                Audio = _mapper.Map<List<AudioDto>>(profile.AudioFiles),
                Video = _mapper.Map<List<VideoDto>>(profile.VideoFiles),
                Photos = _mapper.Map<List<PhotoDto>>(profile.Photos)
            };
        }
    }
}