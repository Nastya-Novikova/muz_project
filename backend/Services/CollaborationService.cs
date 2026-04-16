using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Favorites;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;
using FluentValidation;

namespace backend.Services;

public class CollaborationService(
    ICollaborationSuggestionRepository suggestionRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    INotificationService notificationService,
    IEntityExistenceService existenceService,
    IValidator<SendSuggestionRequest> suggestionValidator) : ICollaborationService
{
    private readonly ICollaborationSuggestionRepository _suggestionRepository = suggestionRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IEntityExistenceService _existenceService = existenceService;
    private readonly IValidator<SendSuggestionRequest> _suggestionValidator = suggestionValidator;

    public async Task<Result> SendSuggestionAsync(Guid fromUserId, Guid toProfileId, string? message)
    {
        var request = new SendSuggestionRequest { ToProfileId = toProfileId, Message = message };
        var validationResult = await _suggestionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToErrorString());
        }

        var fromUserValidation = await _existenceService.GetUserWithProfileAsync(fromUserId);
        if (!fromUserValidation.IsSuccess)
            return Result.Failure(fromUserValidation.Error);
        var fromUser = fromUserValidation.Value;

        var toProfileValidation = await _existenceService.GetMusicianProfileAsync(toProfileId);
        if (!toProfileValidation.IsSuccess)
            return Result.Failure(toProfileValidation.Error);
        var toProfile = toProfileValidation.Value;

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
                ["suggestionId"] = suggestion.Id,
                ["message"] = suggestion.Message ?? string.Empty
            });

        return Result.Success();
    }

    public async Task<Result<PagedResult<SuggestionDto>>> GetReceivedAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc)
    {
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<PagedResult<SuggestionDto>>.Failure(userResult.Error);
        var user = userResult.Value;

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
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<PagedResult<SuggestionDto>>.Failure(userResult.Error);
        var user = userResult.Value;

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
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<bool>.Failure(userResult.Error);
        var user = userResult.Value;

        var sent = await _suggestionRepository.GetSentAsync(user.MusicianProfile.Id, 1, 1);
        return Result<bool>.Success(sent.Any(s => s.ToProfileId == collaboratedProfileId));
    }
}