using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCitiesQuery"/>.
    /// </summary>
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, List<LookupItemDto>>
    {
        private readonly IReferenceDataReadRepository _referenceRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="referenceRepository">Репозиторий справочных данных.</param>
        public GetCitiesQueryHandler(IReferenceDataReadRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            return await _referenceRepository.GetCitiesAsync(cancellationToken);
        }
    }
}