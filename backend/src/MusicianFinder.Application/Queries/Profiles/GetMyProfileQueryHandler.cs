using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyProfileQuery"/>.
    /// </summary>
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMyProfileQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        /// <param name="cache">Сервис кеша.</param>
        public GetMyProfileQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper, ICacheService cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"profile:me:{_currentUserService.UserId}";
            var cached = await _cache.GetAsync<ProfileDto>(cacheKey);
            if (cached != null) return cached;

            var dto = await _dbContext.Profiles
                .AsNoTracking()
                .Where(p => p.Id == _currentUserService.UserId && !p.IsDeleted)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            dto.IsMyProfile = true;
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
            return dto;
        }
    }
}