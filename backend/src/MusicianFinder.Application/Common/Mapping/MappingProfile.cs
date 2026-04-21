using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.Application.Features.Events.DTOs;
using MusicianFinder.Application.Features.Profiles.DTOs;
using MusicianFinder.Application.Features.Collaborations.DTOs;
using MusicianFinder.Application.Features.Favorites.DTOs;
using MusicianFinder.Application.Features.Notifications.DTOs;
using MusicianFinder.Application.Features.Uploads.DTOs;
using MusicianFinder.Application.Features.Metadata.DTOs;
using MusicianFinder.Application.Features.Auth.DTOs;
using MusicianFinder.Application.Features.Profiles.GetMedia;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicianFinder.Application.Common.Mapping
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
            // Справочники
            CreateMap<City, LookupItemDto>();
            CreateMap<Region, LookupItemDto>();
            CreateMap<Genre, LookupItemDto>();
            CreateMap<MusicalSpecialty, LookupItemDto>();
            CreateMap<CollaborationGoal, LookupItemDto>();

            // Профиль музыканта
            CreateMap<MusicianProfile, ProfileDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.NotifyByEmail, opt => opt.MapFrom(src => src.NotifyByEmail))
                .ForMember(dest => dest.NotifyByVk, opt => opt.MapFrom(src => src.NotifyByVk));

            CreateMap<MusicianProfile, ProfileShortDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            // Избранное
            CreateMap<MusicianProfile, FavoriteProfileDto>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.AddedAt, opt => opt.Ignore());

            // Предложения о сотрудничестве
            CreateMap<CollaborationSuggestion, SuggestionDto>();

            // Уведомления
            CreateMap<Notification, NotificationDto>();

            // Мероприятия
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.CurrentParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.IsRegistered, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorFullName, opt => opt.MapFrom(src => src.CreatorProfile != null ? src.CreatorProfile.FullName : string.Empty))
                .ForMember(dest => dest.CreatorAvatarUrl, opt => opt.MapFrom(src => src.CreatorProfile != null ? src.CreatorProfile.AvatarUrl : null));

            // Портфолио
            CreateMap<PortfolioAudio, AudioDto>();
            CreateMap<PortfolioVideo, VideoDto>();
            CreateMap<PortfolioPhoto, PhotoDto>();

            CreateMap<PortfolioAudio, UploadResultDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioVideo, UploadResultDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioPhoto, UploadResultDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Пользователь
            CreateMap<User, UserDto>();
        }
    }
}