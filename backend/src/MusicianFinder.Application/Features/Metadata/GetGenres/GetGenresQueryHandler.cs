using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetGenres
{
    /// <summary>
    /// Обработчик запроса <see cref="GetGenresQuery"/>.
    /// </summary>
    public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, List<LookupItemDto>>
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetGenresQueryHandler"/>.
        /// </summary>
        /// <param name="genreRepository">Репозиторий жанров.</param>
        /// <param name="mapper">Маппер.</param>
        public GetGenresQueryHandler(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
        {
            var genres = await _genreRepository.GetAllAsync(request.Query, request.SortBy, request.SortDesc);
            return _mapper.Map<List<LookupItemDto>>(genres);
        }
    }
}