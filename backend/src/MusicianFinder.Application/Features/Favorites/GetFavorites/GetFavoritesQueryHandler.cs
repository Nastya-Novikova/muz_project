using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Favorites.DTOs;

namespace MusicianFinder.Application.Features.Favorites.GetFavorites
{
    /// <summary>
    /// Обработчик запроса <see cref="GetFavoritesQuery"/>.
    /// </summary>
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PagedResult<FavoriteProfileDto>>
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetFavoritesQueryHandler"/>.
        /// </summary>
        /// <param name="favoriteRepository">Репозиторий избранного.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetFavoritesQueryHandler(
            IFavoriteRepository favoriteRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<FavoriteProfileDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var items = await _favoriteRepository.GetFavoritesByUserIdAsync(_currentUserService.UserId, request.Page, request.Limit);
            var total = await _favoriteRepository.CountFavoritesByUserIdAsync(_currentUserService.UserId);

            var dtos = _mapper.Map<List<FavoriteProfileDto>>(items);

            return new PagedResult<FavoriteProfileDto>
            {
                Items = dtos,
                Total = total,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}