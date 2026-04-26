using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис получения информации о текущем пользователе из JWT-токена.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CurrentUserService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Аксессор HttpContext.</param>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public Guid UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("userId");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        /// <inheritdoc />
        public string Email
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("email");
                return claim?.Value ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public string Role
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
                return claim?.Value ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}