using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;

namespace backend.Services;

public class CollaborationService : ICollaborationService
{
    private readonly ICollaborationSuggestionRepository _suggestionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CollaborationService(
        ICollaborationSuggestionRepository suggestionRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _suggestionRepository = suggestionRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Result> SendSuggestionAsync(Guid fromUserId, Guid toProfileId, string? message)
    {
        var fromUser = await _userRepository.GetByIdAsync(fromUserId);
        if (fromUser?.MusicianProfile == null)
            return Result.Failure("Sender profile not found");

        var toProfile = await _profileRepository.GetByIdAsync(toProfileId);
        if (toProfile == null)
            return Result.Failure("Recipient profile not found");

        var suggestion = new CollaborationSuggestion
        {
            Id = Guid.NewGuid(),
            FromProfileId = fromUser.MusicianProfile.Id,
            ToProfileId = toProfile.Id,
            Message = message ?? string.Empty,
            Status = "pending"
        };

        await _suggestionRepository.AddAsync(suggestion);
        await _unitOfWork.SaveChangesAsync();

        // Отправляем уведомление профилю получателя
        await _notificationService.SendNotificationToProfileAsync(
            toProfile.Id,
            NotificationType.CollaborationReceived,
            new Dictionary<string, object>
            {
                ["fromProfileName"] = fromUser.MusicianProfile.FullName,
                ["suggestionId"] = suggestion.Id
            });

        return Result.Success();
    }

    public async Task<Result<PagedResult<SuggestionDto>>> GetReceivedAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<PagedResult<SuggestionDto>>.Failure("Profile not found");

        var suggestions = await _suggestionRepository.GetReceivedAsync(user.MusicianProfile.Id, page, limit, sortBy, sortDesc);
        var total = suggestions.Count;

        var dtos = _mapper.Map<List<SuggestionDto>>(suggestions);
        var result = new PagedResult<SuggestionDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            Limit = limit
        };
        return Result<PagedResult<SuggestionDto>>.Success(result);
    }

    public async Task<Result<PagedResult<SuggestionDto>>> GetSentAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<PagedResult<SuggestionDto>>.Failure("Profile not found");

        var suggestions = await _suggestionRepository.GetSentAsync(user.MusicianProfile.Id, page, limit, sortBy, sortDesc);
        var total = suggestions.Count;

        var dtos = _mapper.Map<List<SuggestionDto>>(suggestions);
        var result = new PagedResult<SuggestionDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            Limit = limit
        };
        return Result<PagedResult<SuggestionDto>>.Success(result);
    }

    public async Task<Result<bool>> IsCollaboratedAsync(Guid userId, Guid collaboratedProfileId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<bool>.Success(false);

        var sent = await _suggestionRepository.GetSentAsync(user.MusicianProfile.Id, 1, 1);
        return Result<bool>.Success(sent.Any(s => s.ToProfileId == collaboratedProfileId));
    }
}