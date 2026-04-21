using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Features.Auth.DTOs;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Auth.Login
{
    /// <summary>
    /// Обработчик команды <see cref="LoginCommand"/>.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationCodeRepository _codeRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LoginCommandHandler"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="codeRepository">Репозиторий кодов подтверждения.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public LoginCommandHandler(
            IUserRepository userRepository,
            IEmailVerificationCodeRepository codeRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var codeRecord = await _codeRepository.GetByCodeAndEmailAsync(request.Code, request.Email);
            if (codeRecord == null || codeRecord.IsExpired(TimeSpan.FromMinutes(10)))
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.Code), "Недействительный или истёкший код.") });

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                user = new User(request.Email);
                await _userRepository.AddAsync(user);
            }

            await _codeRepository.MarkAsUsedAsync(codeRecord.Id);

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