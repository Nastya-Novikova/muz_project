using AutoMapper;
using backend.Models.Classes;
using backend.Models.DTOs;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Favorites;
using backend.Models.DTOs.Media;
using backend.Models.DTOs.Profiles;
using backend.Models.DTOs.Uploads;

namespace backend.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Базовые маппинги для справочников
            CreateMap<Genre, LookupItemDto>();
            CreateMap<MusicalSpecialty, LookupItemDto>();
            CreateMap<CollaborationGoal, LookupItemDto>();
            CreateMap<City, LookupItemDto>();

            // MusicianProfile → ProfileDto
            CreateMap<MusicianProfile, ProfileDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            // MusicianProfile → FavoriteProfileDto
            CreateMap<MusicianProfile, FavoriteProfileDto>()
                .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.AddedAt, opt => opt.Ignore());

            // CollaborationSuggestion → SuggestionDto
            CreateMap<CollaborationSuggestion, SuggestionDto>();
            CreateMap<MusicianProfile, ProfileShortDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            // Портфолио
            CreateMap<PortfolioAudio, AudioDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl));
            CreateMap<PortfolioVideo, VideoDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl));
            CreateMap<PortfolioPhoto, PhotoDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl));

            CreateMap<PortfolioAudio, UploadResultDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioVideo, UploadResultDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<PortfolioPhoto, UploadResultDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // User → UserDto
            CreateMap<User, UserDto>();
        }
    }
}