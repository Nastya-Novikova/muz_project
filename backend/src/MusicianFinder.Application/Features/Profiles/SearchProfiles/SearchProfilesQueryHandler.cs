using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Profiles.DTOs;

namespace MusicianFinder.Application.Features.Profiles.SearchProfiles
{
    /// <summary>
    /// Обработчик запроса <see cref="SearchProfilesQuery"/>.
    /// </summary>
    public class SearchProfilesQueryHandler : IRequestHandler<SearchProfilesQuery, PagedResult<ProfileDto>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SearchProfilesQueryHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="mapper">Маппер.</param>
        public SearchProfilesQueryHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(SearchProfilesQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _profileRepository.SearchAsync(
                request.Query,
                request.CityId,
                request.GenreIds,
                request.SpecialtyIds,
                request.GoalIds,
                request.DesiredGenreIds,
                request.DesiredSpecialtyIds,
                request.LookingFor,
                request.ProfileType,
                request.ExperienceMin,
                request.ExperienceMax,
                request.Page,
                request.Limit,
                request.SortBy,
                request.SortDesc);

            var dtos = _mapper.Map<List<ProfileDto>>(items);

            return new PagedResult<ProfileDto>
            {
                Items = dtos,
                Total = total,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}