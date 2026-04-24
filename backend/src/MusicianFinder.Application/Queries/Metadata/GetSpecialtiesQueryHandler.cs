using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetSpecialtiesQuery"/>.
    /// </summary>
    public class GetSpecialtiesQueryHandler : IRequestHandler<GetSpecialtiesQuery, List<LookupItemDto>>
    {
        private readonly IReferenceDataReadRepository _referenceRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="referenceRepository">Репозиторий справочных данных.</param>
        public GetSpecialtiesQueryHandler(IReferenceDataReadRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetSpecialtiesQuery request, CancellationToken cancellationToken)
        {
            return await _referenceRepository.GetSpecialtiesAsync(cancellationToken);
        }
    }
}