using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для мягкого удаления профиля текущего пользователя.
    /// </summary>
    public class DeleteProfileCommand : IRequest<Unit>, IBaseCommand
    {
        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}