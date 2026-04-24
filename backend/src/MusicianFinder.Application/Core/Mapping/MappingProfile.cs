using AutoMapper;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Core.Mapping
{
    /// <summary>
    /// Профиль AutoMapper для преобразования доменных сущностей в DTO.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MappingProfile"/> и определяет маппинги.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<City, LookupItemDto>();
            CreateMap<Region, LookupItemDto>();
            CreateMap<Genre, LookupItemDto>();
            CreateMap<MusicalSpecialty, LookupItemDto>();
            CreateMap<CollaborationGoal, LookupItemDto>();

            CreateMap<MusicianProfile, ProfileDto>()
                .ForMember(dto => dto.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dto => dto.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dto => dto.Specialties, opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dto => dto.CollaborationGoals, opt => opt.MapFrom(src => src.CollaborationGoals))
                .ForMember(dto => dto.DesiredGenres, opt => opt.MapFrom(src => src.DesiredGenres))
                .ForMember(dto => dto.DesiredSpecialties, opt => opt.MapFrom(src => src.DesiredSpecialties));

            CreateMap<MusicianProfile, ProfileShortDto>()
                .ForMember(dto => dto.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dto => dto.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dto => dto.Specialties, opt => opt.MapFrom(src => src.Specialties));

            CreateMap<Event, EventDto>()
                .ForMember(dto => dto.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dto => dto.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dto => dto.CreatorFullName, opt => opt.MapFrom(src => src.CreatorProfile != null ? src.CreatorProfile.FullName : string.Empty))
                .ForMember(dto => dto.CreatorAvatarUrl, opt => opt.MapFrom(src => src.CreatorProfile != null ? src.CreatorProfile.AvatarUrl : null))
                .ForMember(dto => dto.CurrentParticipants, opt => opt.Ignore())
                .ForMember(dto => dto.IsRegistered, opt => opt.Ignore())
                .ForMember(dto => dto.IsCreator, opt => opt.Ignore());

            CreateMap<PortfolioItem, AudioDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dto => dto.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dto => dto.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dto => dto.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioItem, VideoDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dto => dto.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dto => dto.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dto => dto.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioItem, PhotoDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dto => dto.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dto => dto.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CollaborationSuggestion, SuggestionDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<User, UserDto>();
        }
    }
}