using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateAvatarCommand"/>.
    /// </summary>
    public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, string>
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public UpdateAvatarCommandHandler(
            ICurrentProfileProvider profileProvider,
            IFileStorage fileStorage)
        {
            _profileProvider = profileProvider;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            using var stream = new MemoryStream(request.Content);
            var url = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);
            profile.SetAvatar(url);
            return url;
        }
    }
}