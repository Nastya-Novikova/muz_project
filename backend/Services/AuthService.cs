using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Auth;
using System.Security.Cryptography;
using FluentValidation;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;
using Minio.DataModel.Notification;
using backend.Services.Utils;

namespace backend.Services;

public class AuthService(
    IUserRepository userRepository,
    IEmailVerificationCodeRepository codeRepository,
    IEmailService emailService,
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IValidator<RequestCodeRequest> requestCodeValidator,
    IValidator<LoginRequest> loginValidator) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _config = config;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<RequestCodeRequest> _requestCodeValidator = requestCodeValidator;
    private readonly IValidator<LoginRequest> _loginValidator = loginValidator;

    public async Task<Result?> RequestCodeAsync(string email)
    {
        var request = new RequestCodeRequest { Email = email };
        var validationResult = await _requestCodeValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToErrorString());
        }

        try
        {
            var code = "111111";
            //var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var verificationCode = new EmailVerificationCode
            {
                Email = email,
                Code = code
            };

            await _codeRepository.AddAsync(verificationCode);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendVerificationCodeAsync(email, code);

            return Result.Success();
        }
        catch
        {
            return Result.Failure("The code was not sent");
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string code)
    {
        var request = new LoginRequest { Email = email, Code = code };
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result<AuthResponse>.Failure(validationResult.ToErrorString());
        }

        try
        {
            var codeRecord = await _codeRepository.GetByCodeAndEmailAsync(code, email);
            if (codeRecord == null || DateTime.UtcNow - codeRecord.CreatedAt > TimeSpan.FromMinutes(10))
                return Result<AuthResponse>.Failure("Invalid or expired code");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User { Email = email };
                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            await _codeRepository.MarkAsUsedAsync(codeRecord.Id);
            var token = GenerateJwtToken(user);

            var response = new AuthResponse
            {
                Success = true,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    ProfileCreated = user.ProfileCreated,
                    Role = user.Role
                }
            };

            return Result<AuthResponse>.Success(response);
        }
        catch
        {
            return Result<AuthResponse>.Failure("Login failure");
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}