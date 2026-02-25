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
            // MusicianProfile → ProfileDto
            CreateMap<MusicianProfile, ProfileDto>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.LocalizedName : null))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dest => dest.CollaborationGoals, opt => opt.MapFrom(src => src.CollaborationGoals))
                .ForMember(dest => dest.DesiredGenres, opt => opt.MapFrom(src => src.DesiredGenres))
                .ForMember(dest => dest.DesiredSpecialties, opt => opt.MapFrom(src => src.DesiredSpecialties));

            // Справочники → LookupItemDto
            CreateMap<Genre, LookupItemDto>();
            CreateMap<MusicalSpecialty, LookupItemDto>();
            CreateMap<CollaborationGoal, LookupItemDto>();
            CreateMap<City, LookupItemDto>();

            // Избранное
            CreateMap<MusicianProfile, FavoriteProfileDto>()
                .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.LocalizedName : null))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dest => dest.AddedAt, opt => opt.Ignore()); // будет заполняться отдельно

            // Предложения сотрудничества
            CreateMap<CollaborationSuggestion, SuggestionDto>()
                .ForMember(dest => dest.FromProfile, opt => opt.MapFrom(src => src.FromProfile))
                .ForMember(dest => dest.ToProfile, opt => opt.MapFrom(src => src.ToProfile));

            CreateMap<MusicianProfile, ProfileShortDto>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.LocalizedName : null));

            /*// Портфолио (после перехода на FileUrl)
            CreateMap<PortfolioAudio, AudioDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl ?? ""));
            CreateMap<PortfolioVideo, VideoDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl ?? ""));
            CreateMap<PortfolioPhoto, PhotoDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl ?? ""));*/

            // Пользователь
            CreateMap<User, UserDto>();
        }
    }
}