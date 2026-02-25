using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.User;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return Result<UserDto>.Failure("User not found");
        var dto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(dto);
    }

    public async Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<UserDto>.Failure("User not found");

        // обновление полей (например, ProfileCreated, Role и т.д.)
        if (request.ProfileCreated.HasValue)
            user.ProfileCreated = request.ProfileCreated.Value;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(dto);
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("User not found");

        await _userRepository.SoftDeleteAsync(userId);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

/*public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<JsonDocument?> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : JsonDocument.Parse(JsonSerializer.Serialize(user, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> UpdateProfileAsync(Guid userId, JsonDocument profileJson)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var root = profileJson.RootElement;

            if (root.TryGetProperty("favoriteProfileIds", out var favIds))
            {
                user.FavoriteProfileIds = favIds.EnumerateArray().Select(x => Guid.Parse(x.GetString()!)).ToList();
            }

            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(user, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> DeleteAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            await _userRepository.SoftDeleteAsync(userId);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }
}*/