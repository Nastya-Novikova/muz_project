using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
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

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetProfileByIdQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetProfileByIdQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .Include(nameof(Domain.Entities.MusicianProfile.City))
                .Include(nameof(Domain.Entities.MusicianProfile.Genres))
                .Include(nameof(Domain.Entities.MusicianProfile.Specialties))
                .Include(nameof(Domain.Entities.MusicianProfile.CollaborationGoals))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredGenres))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredSpecialties))
                .FirstOrDefaultAsync(p => p.Id == request.ProfileId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.MusicianProfile), request.ProfileId);

            var dto = _mapper.Map<ProfileDto>(profile);

            if (_currentUserService.IsAuthenticated)
            {
                var currentProfile = await _dbContext.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken);

                if (currentProfile != null)
                {
                    dto.IsMyProfile = currentProfile.Id == profile.Id;

                    dto.IsFavorite = await _dbContext.Users
                        .Where(u => u.Id == _currentUserService.UserId)
                        .SelectMany(u => u.Favorites)
                        .AnyAsync(f => f.ProfileId == profile.Id, cancellationToken);

                    dto.IsCollaborated = await _dbContext.CollaborationSuggestions
                        .AnyAsync(s => s.FromProfileId == currentProfile.Id && s.ToProfileId == profile.Id, cancellationToken);
                }
            }

            return dto;
        }
    }
}