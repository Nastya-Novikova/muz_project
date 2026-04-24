using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Команда для добавления профиля в избранное текущего пользователя.
    /// </summary>
    public class AddFavoriteCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор профиля, который нужно добавить в избранное.
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}