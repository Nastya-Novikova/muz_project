using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
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
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventByIdQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        /// <param name="cache">Сервис кеша.</param>
        public GetEventByIdQueryHandler(IReadDbContext dbContext, IMapper mapper, ICacheService cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"event:{request.EventId}";
            var cached = await _cache.GetAsync<EventDto>(cacheKey);
            if (cached != null)
            {
                // Дополнительные вычисляемые поля могут быть неактуальны, но для простоты возвращаем как есть
                return cached;
            }

            var dto = await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.Id == request.EventId && !e.IsDeleted)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Event), request.EventId);

            dto.CurrentParticipants = await _dbContext.Events
                .Where(e => e.Id == request.EventId)
                .SelectMany(e => e.Registrations)
                .CountAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(2));

            return dto;
        }
    }
}