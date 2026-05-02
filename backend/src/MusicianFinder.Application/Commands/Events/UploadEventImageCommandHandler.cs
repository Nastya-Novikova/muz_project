using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UploadEventImageCommand"/>.
    /// </summary>
    public class UploadEventImageCommandHandler : IRequestHandler<UploadEventImageCommand, string>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        /// <param name="fileStorage">Файловое хранилище.</param>
        public UploadEventImageCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider,
            IFileStorage fileStorage)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            using var stream = new MemoryStream(request.Content);
            var imageUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);
            @event.SetImage(imageUrl, profile.Id);
            return imageUrl;
        }
    }
}