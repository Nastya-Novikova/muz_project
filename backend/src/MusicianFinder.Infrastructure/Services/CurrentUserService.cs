using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для получения информации о текущем аутентифицированном пользователе.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CurrentUserService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Аксессор HTTP-контекста.</param>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public Guid UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
            }
        }

        /// <inheritdoc />
        public string Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public string Role
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}