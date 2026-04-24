using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Команда для удаления профиля из избранного.
    /// </summary>
    public class RemoveFavoriteCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор профиля, удаляемого из избранного.
        /// </summary>
        public Guid TargetProfileId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}