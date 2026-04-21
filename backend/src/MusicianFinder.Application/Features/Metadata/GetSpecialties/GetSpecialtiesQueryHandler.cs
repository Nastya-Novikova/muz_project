using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetSpecialties
{
    /// <summary>
    /// Обработчик запроса <see cref="GetSpecialtiesQuery"/>.
    /// </summary>
    public class GetSpecialtiesQueryHandler : IRequestHandler<GetSpecialtiesQuery, List<LookupItemDto>>
    {
        private readonly IMusicalSpecialtyRepository _specialtyRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetSpecialtiesQueryHandler"/>.
        /// </summary>
        /// <param name="specialtyRepository">Репозиторий специальностей.</param>
        /// <param name="mapper">Маппер.</param>
        public GetSpecialtiesQueryHandler(IMusicalSpecialtyRepository specialtyRepository, IMapper mapper)
        {
            _specialtyRepository = specialtyRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetSpecialtiesQuery request, CancellationToken cancellationToken)
        {
            var specialties = await _specialtyRepository.GetAllAsync(request.Query, request.SortBy, request.SortDesc);
            return _mapper.Map<List<LookupItemDto>>(specialties);
        }
    }
}