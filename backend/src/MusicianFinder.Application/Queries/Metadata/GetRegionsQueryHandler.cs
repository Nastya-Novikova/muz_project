using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetRegionsQuery"/>.
    /// </summary>
    public class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<LookupItemDto>>
    {
        private readonly IReferenceDataReadRepository _referenceRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="referenceRepository">Репозиторий справочных данных.</param>
        public GetRegionsQueryHandler(IReferenceDataReadRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            return await _referenceRepository.GetRegionsAsync(cancellationToken);
        }
    }
}