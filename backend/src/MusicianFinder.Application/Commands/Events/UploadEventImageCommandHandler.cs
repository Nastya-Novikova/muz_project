using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UploadEventImageCommand"/>.
    /// </summary>
    public class UploadEventImageCommandHandler : IRequestHandler<UploadEventImageCommand, string>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public UploadEventImageCommandHandler(
            IEventRepository eventRepository,
            ICurrentUserService currentUser,
            IMusicianProfileRepository profileRepository,
            IFileStorage fileStorage)
        {
            _eventRepository = eventRepository;
            _currentUser = currentUser;
            _profileRepository = profileRepository;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Мероприятие не найдено.");

            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            using var stream = new MemoryStream(request.Content);
            var imageUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);
            @event.SetImage(imageUrl, profile.Id);
            return imageUrl;
        }
    }
}