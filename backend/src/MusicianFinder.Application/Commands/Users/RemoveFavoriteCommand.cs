using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Команда для удаления профиля из избранного текущего пользователя.
    /// </summary>
    public class RemoveFavoriteCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор профиля, который нужно удалить из избранного.
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}