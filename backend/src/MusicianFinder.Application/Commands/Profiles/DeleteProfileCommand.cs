using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для мягкого удаления профиля.
    /// </summary>
    public class DeleteProfileCommand : ICommand<Unit>, IBaseCommand
    {
        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}