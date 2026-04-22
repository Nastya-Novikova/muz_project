using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventByIdQuery"/>.
    /// </summary>
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventByIdQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetEventByIdQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _dbContext.Events
                .AsNoTracking()
                .Include(nameof(Domain.Entities.Event.Region))
                .Include(nameof(Domain.Entities.Event.City))
                .Include(nameof(Domain.Entities.Event.CreatorProfile))
                .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Event), request.EventId);

            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = await _dbContext.Events
                .Where(e => e.Id == request.EventId)
                .SelectMany(e => e.Registrations)
                .CountAsync(cancellationToken);

            if (_currentUserService.IsAuthenticated)
            {
                var profile = await _dbContext.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken);
                if (profile != null)
                {
                    dto.IsRegistered = await _dbContext.Events
                        .Where(e => e.Id == request.EventId)
                        .SelectMany(e => e.Registrations)
                        .AnyAsync(r => r.ProfileId == profile.Id, cancellationToken);
                    dto.IsCreator = eventEntity.CreatorProfileId == profile.Id;
                }
            }

            return dto;
        }
    }
}   