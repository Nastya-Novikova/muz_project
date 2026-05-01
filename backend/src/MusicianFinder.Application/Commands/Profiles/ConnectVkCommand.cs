using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Profiles
{
    public class ConnectVkCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Код авторизации OAuth.
        /// </summary>
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Верификатор кода (PKCE).
        /// </summary>
        public string CodeVerifier { get; set; } = string.Empty;
        
        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}