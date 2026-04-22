using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMediaQuery"/>.
    /// </summary>
    public class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, MediaDto>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMediaQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMediaQueryHandler(IReadDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<MediaDto> Handle(GetMediaQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .Include(nameof(Domain.Entities.MusicianProfile.PortfolioItems))
                .FirstOrDefaultAsync(p => p.Id == request.ProfileId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            return new MediaDto
            {
                Audio = _mapper.Map<List<AudioDto>>(profile.PortfolioItems.Where(x => x.Type == MediaType.Audio).ToList()),
                Video = _mapper.Map<List<VideoDto>>(profile.PortfolioItems.Where(x => x.Type == MediaType.Video).ToList()),
                Photos = _mapper.Map<List<PhotoDto>>(profile.PortfolioItems.Where(x => x.Type == MediaType.Photo).ToList())
            };
        }
    }
}