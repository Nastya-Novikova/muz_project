using System;
using System.Collections.Generic;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Tests.Shared.Builders
{
    public class MusicianProfileBuilder
    {
        private ProfileType _profileType = ProfileType.Individual;
        private Guid _userId = Guid.NewGuid();
        private ProfileName _fullName = new ProfileName("Test Musician");
        private int? _age = null;
        private int _cityId = 1;
        private PhoneNumber? _phone = null;
        private TelegramHandle? _telegram = null;
        private VkUserId? _vkUserId = null;
        private string? _description = null;
        private string? _avatarUrl = null;
        private int _experience = 0;
        private LookingFor _lookingFor = LookingFor.NotLooking;
        // Уникальный email по умолчанию, чтобы избежать конфликтов уникальности
        private string _email = $"test_{Guid.NewGuid()}@example.com";
        private bool _notifyByEmail = true;
        private bool _notifyByVk = false;
        private readonly List<GenreId> _genreIds = new();
        private readonly List<SpecialtyId> _specialtyIds = new();
        private readonly List<CollaborationGoalId> _collaborationGoalIds = new();
        private readonly List<GenreId> _desiredGenreIds = new();
        private readonly List<SpecialtyId> _desiredSpecialtyIds = new();

        public MusicianProfileBuilder WithProfileType(ProfileType type) { _profileType = type; return this; }
        public MusicianProfileBuilder WithUserId(Guid userId) { _userId = userId; return this; }
        public MusicianProfileBuilder WithFullName(string fullName) { _fullName = new ProfileName(fullName); return this; }
        public MusicianProfileBuilder WithAge(int? age) { _age = age; return this; }
        public MusicianProfileBuilder WithCityId(int cityId) { _cityId = cityId; return this; }
        public MusicianProfileBuilder WithPhone(string phone) { _phone = new PhoneNumber(phone); return this; }
        public MusicianProfileBuilder WithTelegram(string telegram) { _telegram = new TelegramHandle(telegram); return this; }
        public MusicianProfileBuilder WithVkUserId(string vkUserId) { _vkUserId = new VkUserId(vkUserId); return this; }
        public MusicianProfileBuilder WithDescription(string? description) { _description = description; return this; }
        public MusicianProfileBuilder WithAvatarUrl(string? avatarUrl) { _avatarUrl = avatarUrl; return this; }
        public MusicianProfileBuilder WithExperience(int experience) { _experience = experience; return this; }
        public MusicianProfileBuilder WithLookingFor(LookingFor lookingFor) { _lookingFor = lookingFor; return this; }
        public MusicianProfileBuilder WithEmail(string email) { _email = email; return this; }
        public MusicianProfileBuilder WithNotifyByEmail(bool notify) { _notifyByEmail = notify; return this; }
        public MusicianProfileBuilder WithNotifyByVk(bool notify) { _notifyByVk = notify; return this; }
        public MusicianProfileBuilder AddGenre(int genreId) { _genreIds.Add(new GenreId(genreId)); return this; }
        public MusicianProfileBuilder AddSpecialty(int specialtyId) { _specialtyIds.Add(new SpecialtyId(specialtyId)); return this; }
        public MusicianProfileBuilder AddCollaborationGoal(int goalId) { _collaborationGoalIds.Add(new CollaborationGoalId(goalId)); return this; }
        public MusicianProfileBuilder AddDesiredGenre(int genreId) { _desiredGenreIds.Add(new GenreId(genreId)); return this; }
        public MusicianProfileBuilder AddDesiredSpecialty(int specialtyId) { _desiredSpecialtyIds.Add(new SpecialtyId(specialtyId)); return this; }

        public MusicianProfile Build()
        {
            var profile = MusicianProfile.Create(_userId, _fullName, _cityId, _email, _profileType);
            profile.UpdateCoreInfo(_fullName, _age, _description, _cityId);
            profile.UpdateContacts(_phone, _telegram);
            if (_vkUserId != null) profile.SetVkUserId(_vkUserId);
            profile.SetAvatar(_avatarUrl);
            profile.SetExperience(_experience);
            profile.SetLookingFor(_lookingFor);
            profile.UpdateNotificationPreferences(_notifyByEmail, _notifyByVk);
            profile.SetGenres(_genreIds);
            profile.SetSpecialties(_specialtyIds);
            profile.SetCollaborationGoals(_collaborationGoalIds);
            profile.SetDesiredGenres(_desiredGenreIds);
            profile.SetDesiredSpecialties(_desiredSpecialtyIds);
            return profile;
        }
    }
}