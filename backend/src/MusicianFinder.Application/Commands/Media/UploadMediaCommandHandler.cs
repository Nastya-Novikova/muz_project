using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="UploadMediaCommand"/>.
    /// </summary>
    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, Guid>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public UploadMediaCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser,
            IFileStorage fileStorage)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            using var stream = new MemoryStream(request.Content);
            var fileUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);

            var portfolioItem = new PortfolioItem(request.Title, fileUrl, request.ContentType, request.Type);
            portfolioItem.Description = request.Description;
            profile.AddPortfolioItem(portfolioItem);

            return portfolioItem.Id;
        }
    }
}