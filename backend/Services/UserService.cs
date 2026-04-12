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