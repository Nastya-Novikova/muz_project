using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
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

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMyProfileQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMyProfileQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .Include(nameof(Domain.Entities.MusicianProfile.City))
                .Include(nameof(Domain.Entities.MusicianProfile.Genres))
                .Include(nameof(Domain.Entities.MusicianProfile.Specialties))
                .Include(nameof(Domain.Entities.MusicianProfile.CollaborationGoals))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredGenres))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredSpecialties))
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var dto = _mapper.Map<ProfileDto>(profile);
            dto.IsMyProfile = true;

            return dto;
        }
    }
}