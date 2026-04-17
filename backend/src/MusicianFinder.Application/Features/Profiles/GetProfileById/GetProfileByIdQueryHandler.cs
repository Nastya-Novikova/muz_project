using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Profiles.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicianFinder.Application.Features.Profiles.GetProfileById
{
    /// <summary>
    /// Обработчик запроса <see cref="GetProfileByIdQuery"/>.
    /// </summary>
    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetProfileByIdQueryHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="mapper">Маппер.</param>
        public GetProfileByIdQueryHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.ProfileId);
            if (profile == null)
                throw new NotFoundException(nameof(Profile), request.ProfileId);

            return _mapper.Map<ProfileDto>(profile);
        }
    }
}