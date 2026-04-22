using MediatR;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Команда для удаления профиля из избранного текущего пользователя.
    /// </summary>
    public class RemoveFavoriteCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор профиля, который нужно удалить из избранного.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}