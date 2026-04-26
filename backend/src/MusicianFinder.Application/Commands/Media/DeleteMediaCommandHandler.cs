using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteMediaCommand"/>.
    /// </summary>
    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, Unit>
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
        public DeleteMediaCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser,
            IFileStorage fileStorage)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            var item = profile.Portfolio.FirstOrDefault(i => i.Id == request.MediaId)
                ?? throw new Application.Core.Exceptions.NotFoundException("Медиа не найдено.");

            await _fileStorage.DeleteFileAsync(item.FileUrl);
            profile.RemovePortfolioItem(request.MediaId);
            return Unit.Value;
        }
    }
}