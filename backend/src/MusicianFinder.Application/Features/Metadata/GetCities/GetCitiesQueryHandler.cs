using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetCities
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCitiesQuery"/>.
    /// </summary>
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, List<LookupItemDto>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetCitiesQueryHandler"/>.
        /// </summary>
        /// <param name="cityRepository">Репозиторий городов.</param>
        /// <param name="mapper">Маппер.</param>
        public GetCitiesQueryHandler(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            var cities = await _cityRepository.GetAllAsync(request.Query, request.SortBy, request.SortDesc);
            return _mapper.Map<List<LookupItemDto>>(cities);
        }
    }
}