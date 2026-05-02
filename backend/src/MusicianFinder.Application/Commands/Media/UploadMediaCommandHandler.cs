using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="UploadMediaCommand"/>.
    /// </summary>
    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, Guid>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="profileProvider">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public UploadMediaCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentProfileProvider profileProvider,
            IFileStorage fileStorage)
        {
            _profileRepository = profileRepository;
            _profileProvider = profileProvider;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            using var stream = new MemoryStream(request.Content);
            var fileUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);

            var portfolioItem = new PortfolioItem(request.Title, fileUrl, request.ContentType, request.Type);
            portfolioItem.SetDescription(request.Description);

            await _profileRepository.ExecuteAndTrackNewOwnedAsync<PortfolioItem>(
                profile.UserId,
                p => p.AddPortfolioItem(portfolioItem),
                cancellationToken);

            return portfolioItem.Id;
        }
    }
}