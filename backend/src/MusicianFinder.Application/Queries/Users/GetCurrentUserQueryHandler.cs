using MediatR;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Users
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCurrentUserQuery"/>.
    /// </summary>
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="userReadRepository">Репозиторий для чтения пользователей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetCurrentUserQueryHandler(IUserReadRepository userReadRepository, ICurrentUserService currentUser)
        {
            _userReadRepository = userReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userDto = await _userReadRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");
            return userDto;
        }
    }
}