using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using ValidationException = MusicianFinder.Application.Common.Exceptions.ValidationException;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Обработчик команды <see cref="LoginCommand"/>.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LoginCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public LoginCommandHandler(IReadDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var codeRecord = await _dbContext.EmailVerificationCodes
                .Where(c => c.Code == request.Code && c.Email == request.Email && !c.IsUsed)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (codeRecord == null || codeRecord.IsExpired(TimeSpan.FromMinutes(10)))
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.Code), "Недействительный или истёкший код.") });

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                user = new User(request.Email);
                await ((DbContext)_dbContext).AddAsync(user, cancellationToken);
            }

            codeRecord.MarkAsUsed();
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    ProfileCreated = user.ProfileCreated,
                    Role = user.Role.ToString()
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}