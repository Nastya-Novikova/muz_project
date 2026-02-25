using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;
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

    public CollaborationService(
        ICollaborationSuggestionRepository suggestionRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _suggestionRepository = suggestionRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        return Result.Success();
    }

    public async Task<Result<PagedResult<SuggestionDto>>> GetReceivedAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<PagedResult<SuggestionDto>>.Failure("Profile not found");

        var suggestions = await _suggestionRepository.GetReceivedAsync(user.MusicianProfile.Id, page, limit, sortBy, sortDesc);
        // TODO: получить общее количество
        var total = suggestions.Count; // временно, нужно добавить метод CountReceived

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
        var total = suggestions.Count; // временно

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

/*public class CollaborationService : ICollaborationService
{
    private readonly ICollaborationSuggestionRepository _suggestionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;

    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public CollaborationService(
        ICollaborationSuggestionRepository suggestionRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository)
    {
        _suggestionRepository = suggestionRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public async Task<JsonDocument?> SendSuggestionAsync(Guid userId, Guid toProfileId, string? message)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var fromProfile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            var toProfile = await _profileRepository.GetByIdAsync(toProfileId);
            if (fromProfile == null || toProfile == null) return null;

            var suggestion = new CollaborationSuggestion
            {
                Id = Guid.NewGuid(),
                FromProfileId = fromProfile.Id,
                ToProfileId = toProfile.Id,
                Message = message ?? string.Empty
            };

            await _suggestionRepository.AddAsync(suggestion);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true, collaborationId = suggestion.Id }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetReceivedAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;
            var sortBy = root.TryGetProperty("sortBy", out var sb) ? sb.GetString() : "createdAt";
            var sortDesc = root.TryGetProperty("sortDesc", out var sd) ? sd.GetBoolean() : true;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null) return null;
            var result = await _suggestionRepository.GetReceivedAsync(profile.Id, page, limit, sortBy, sortDesc);
            var suggestions = result.Select(suggestion =>
            {
                if (suggestion == null) return null;
                var profile = _profileRepository.GetByIdAsync(suggestion.FromProfileId).Result;
                if (profile == null) return null;
                return new
                {
                    FromProfile = new {
                    profile.Id,
                    profile.FullName,
                    profile.Description,
                    profile.Phone,
                    profile.Telegram,
                    profile.City,
                    profile.Experience,
                    profile.Age,
                    profile.Avatar,
                    Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                    Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                    CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g))
                    },
                    suggestion.Message,
                    suggestion.Status,
                    suggestion.CreatedAt
                };
            });
            return JsonDocument.Parse(JsonSerializer.Serialize(new { suggestions }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetSentAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;
            var sortBy = root.TryGetProperty("sortBy", out var sb) ? sb.GetString() : "createdAt";
            var sortDesc = root.TryGetProperty("sortDesc", out var sd) ? sd.GetBoolean() : true;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null) return null;
            var result = await _suggestionRepository.GetSentAsync(profile.Id, page, limit, sortBy, sortDesc);
            var suggestions = result.Select(suggestion =>
            {
                if (suggestion == null) return null;
                var profile = _profileRepository.GetByIdAsync(suggestion.ToProfileId).Result;
                if (profile == null) return null;
                return new
                {
                    ToProfile = new
                    {
                        profile.Id,
                        profile.FullName,
                        profile.Description,
                        profile.Phone,
                        profile.Telegram,
                        profile.City,
                        profile.Experience,
                        profile.Age,
                        profile.Avatar,
                        Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                        Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                        CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g))
                    },
                    suggestion.Message,
                    suggestion.Status,
                    suggestion.CreatedAt
                };
            });
            return JsonDocument.Parse(JsonSerializer.Serialize(new { suggestions }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsCollaboratedAsync(Guid userId, Guid collaboratedProfileId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return false;
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            var profileCollaborated = await _profileRepository.GetByIdAsync(collaboratedProfileId);
            if (profileCollaborated == null || profile == null) return false;

            return (await _suggestionRepository.GetSentAsync(profile.Id)).Select(o => o.ToProfile).Contains(profileCollaborated) == true;
        }
        catch
        {
            return false;
        }
    }
}*/