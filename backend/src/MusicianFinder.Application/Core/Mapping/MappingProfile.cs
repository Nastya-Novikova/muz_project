using AutoMapper;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;

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
            // Value Objects → строки
            CreateMap<ProfileName, string>().ConvertUsing(src => src.Value);
            CreateMap<string, ProfileName>().ConvertUsing(src => new ProfileName(src));
            CreateMap<PhoneNumber, string>().ConvertUsing(src => src.Value);
            CreateMap<string, PhoneNumber>().ConvertUsing(src => new PhoneNumber(src));
            CreateMap<TelegramHandle, string>().ConvertUsing(src => src.Value);
            CreateMap<string, TelegramHandle>().ConvertUsing(src => new TelegramHandle(src));
            CreateMap<EventTitle, string>().ConvertUsing(src => src.Value);
            CreateMap<string, EventTitle>().ConvertUsing(src => new EventTitle(src));

            // Справочники
            CreateMap<City, LookupItemDto>();
            CreateMap<Region, LookupItemDto>();
            CreateMap<Genre, LookupItemDto>();
            CreateMap<MusicalSpecialty, LookupItemDto>();
            CreateMap<CollaborationGoal, LookupItemDto>();

            // Профили
            CreateMap<MusicianProfile, ProfileDto>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(src => src.FullName.Value))
                .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone != null ? src.Phone.Value : null))
                .ForMember(dto => dto.Telegram, opt => opt.MapFrom(src => src.Telegram != null ? src.Telegram.Value : null))
                .ForMember(dto => dto.City, opt => opt.Ignore())
                .ForMember(dto => dto.Genres, opt => opt.Ignore())
                .ForMember(dto => dto.Specialties, opt => opt.Ignore())
                .ForMember(dto => dto.CollaborationGoals, opt => opt.Ignore())
                .ForMember(dto => dto.DesiredGenres, opt => opt.Ignore())
                .ForMember(dto => dto.DesiredSpecialties, opt => opt.Ignore())
                .ForMember(dto => dto.IsMyProfile, opt => opt.Ignore())
                .ForMember(dto => dto.IsFavorite, opt => opt.Ignore())
                .ForMember(dto => dto.IsCollaborated, opt => opt.Ignore());

            CreateMap<MusicianProfile, ProfileShortDto>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(src => src.FullName.Value))
                .ForMember(dto => dto.City, opt => opt.Ignore())
                .ForMember(dto => dto.Genres, opt => opt.Ignore())
                .ForMember(dto => dto.Specialties, opt => opt.Ignore());

            // Мероприятия
            CreateMap<Event, EventDto>()
                .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title.Value))
                .ForMember(dto => dto.Region, opt => opt.Ignore())
                .ForMember(dto => dto.City, opt => opt.Ignore())
                .ForMember(dto => dto.CreatorFullName, opt => opt.Ignore())
                .ForMember(dto => dto.CreatorAvatarUrl, opt => opt.Ignore())
                .ForMember(dto => dto.CurrentParticipants, opt => opt.Ignore())
                .ForMember(dto => dto.IsRegistered, opt => opt.Ignore())
                .ForMember(dto => dto.IsCreator, opt => opt.Ignore());

            // Медиа
            CreateMap<PortfolioItem, AudioDto>();
            CreateMap<PortfolioItem, VideoDto>();
            CreateMap<PortfolioItem, PhotoDto>();

            // Предложения
            CreateMap<CollaborationSuggestion, SuggestionDto>()
                .ForMember(dto => dto.FromProfile, opt => opt.Ignore())
                .ForMember(dto => dto.ToProfile, opt => opt.Ignore());

            // Уведомления
            CreateMap<Notification, NotificationDto>()
                .ForMember(dto => dto.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dto => dto.EntityType, opt => opt.MapFrom(src => src.EntityType.ToString()));

            // Пользователи
            CreateMap<User, UserDto>()
                .ForMember(dto => dto.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}