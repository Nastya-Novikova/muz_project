using MediatR;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMediaQuery"/>.
    /// </summary>
    public class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, MediaDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileReadRepository">Репозиторий для чтения профилей.</param>
        public GetMediaQueryHandler(IProfileReadRepository profileReadRepository)
        {
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<MediaDto> Handle(GetMediaQuery request, CancellationToken cancellationToken)
        {
            var media = await _profileReadRepository.GetMediaAsync(request.ProfileId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Медиа не найдены.");
            return media;
        }
    }
}