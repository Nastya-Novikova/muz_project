using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetRegions
{
    /// <summary>
    /// Обработчик запроса <see cref="GetRegionsQuery"/>.
    /// </summary>
    public class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<LookupItemDto>>
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetRegionsQueryHandler"/>.
        /// </summary>
        /// <param name="regionRepository">Репозиторий регионов.</param>
        /// <param name="mapper">Маппер.</param>
        public GetRegionsQueryHandler(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _regionRepository.GetAllAsync(request.Query, request.SortBy, request.SortDesc);
            return _mapper.Map<List<LookupItemDto>>(regions);
        }
    }
}