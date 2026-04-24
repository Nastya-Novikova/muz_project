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
    /// Обработчик запроса <see cref="GetProfileByIdQuery"/>.
    /// </summary>
    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetProfileByIdQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        /// <param name="cache">Сервис кеша.</param>
        public GetProfileByIdQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper, ICacheService cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"profile:{request.ProfileId}";
            var cached = await _cache.GetAsync<ProfileDto>(cacheKey);
            if (cached != null) return cached;

            var dto = await _dbContext.Profiles
                .AsNoTracking()
                .Where(p => p.Id == request.ProfileId && !p.IsDeleted)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.MusicianProfile), request.ProfileId);

            if (_currentUserService.IsAuthenticated)
            {
                var currentProfile = await _dbContext.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken);

                if (currentProfile != null)
                {
                    dto.IsMyProfile = currentProfile.Id == dto.Id;
                    dto.IsFavorite = await _dbContext.Users
                        .Where(u => u.Id == _currentUserService.UserId)
                        .SelectMany(u => u.Favorites)
                        .AnyAsync(f => f.ProfileId == dto.Id, cancellationToken);

                    dto.IsCollaborated = await _dbContext.CollaborationSuggestions
                        .AnyAsync(s => s.FromProfileId == currentProfile.Id && s.ToProfileId == dto.Id, cancellationToken);
                }
            }

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
            return dto;
        }
    }
}