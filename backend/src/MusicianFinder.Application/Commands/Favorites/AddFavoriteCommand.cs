using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Команда для добавления профиля в избранное.
    /// </summary>
    public class AddFavoriteCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор профиля, добавляемого в избранное.
        /// </summary>
        public Guid TargetProfileId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}