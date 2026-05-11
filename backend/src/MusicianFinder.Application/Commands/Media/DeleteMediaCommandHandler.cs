using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteMediaCommand"/>.
    /// </summary>
    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, Unit>
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="profileProvider">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public DeleteMediaCommandHandler(
            ICurrentProfileProvider profileProvider,
            IFileStorage fileStorage)
        {
            _profileProvider = profileProvider;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            var item = profile.Portfolio.FirstOrDefault(i => i.Id == request.MediaId)
                ?? throw new NotFoundException("Медиа не найдено.");

            await _fileStorage.DeleteFileAsync(item.FileUrl);
            profile.RemovePortfolioItem(request.MediaId);
            return Unit.Value;
        }
    }
}