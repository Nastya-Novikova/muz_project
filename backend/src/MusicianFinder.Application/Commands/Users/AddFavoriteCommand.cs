using MediatR;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Команда для добавления профиля в избранное текущего пользователя.
    /// </summary>
    public class AddFavoriteCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор профиля, который нужно добавить в избранное.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}