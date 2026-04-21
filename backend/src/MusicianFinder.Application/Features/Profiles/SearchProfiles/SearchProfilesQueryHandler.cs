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
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Features.Profiles.SearchProfiles
{
    /// <summary>
    /// Обработчик запроса <see cref="SearchProfilesQuery"/>.
    /// </summary>
    public class SearchProfilesQueryHandler : IRequestHandler<SearchProfilesQuery, PagedResult<ProfileDto>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICollaborationSuggestionRepository _collaborationSuggestionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SearchProfilesQueryHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="mapper">Маппер.</param>
        public SearchProfilesQueryHandler(IProfileRepository profileRepository, IFavoriteRepository favoriteRepository, ICollaborationSuggestionRepository collaborationSuggestionRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _favoriteRepository = favoriteRepository;
            _collaborationSuggestionRepository = collaborationSuggestionRepository;
            _currentUserService = currentUserService;
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

            var currentUserId = _currentUserService.UserId;
            var currentUserProfile = await _profileRepository.GetByUserIdAsync(currentUserId);

            foreach (var dto in dtos)
            {
                dto.IsMyProfile = currentUserProfile != null && dto.Id == currentUserProfile.Id;

                if (currentUserProfile != null)
                {
                    dto.IsFavorite = await _favoriteRepository.ExistsAsync(currentUserId, dto.Id);
                    dto.IsCollaborated = await _collaborationSuggestionRepository.ExistsAsync(currentUserProfile.Id, dto.Id);
                }
            }

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