using System;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Tests.Shared.Factories
{
    /// <summary>
    /// Фабрика для создания типовых тестовых данных (команды, запросы, DTO).
    /// </summary>
    public static class TestDataFactory
    {
        /// <summary>Создаёт команду запроса кода с указанным email.</summary>
        public static RequestCodeCommand CreateRequestCodeCommand(string email) => new RequestCodeCommand { Email = email };

        /// <summary>Создаёт команду логина с указанными email и кодом.</summary>
        public static LoginCommand CreateLoginCommand(string email, string code = "111111") => new LoginCommand { Email = email, Code = code };

        /// <summary>Создаёт команду создания профиля с минимальными данными.</summary>
        public static CreateProfileCommand CreateMinimalProfileCommand(string fullName, int cityId = 1)
        {
            return new CreateProfileCommand
            {
                FullName = fullName,
                ProfileType = ProfileType.Individual,
                CityId = cityId,
                Experience = 0,
                LookingFor = LookingFor.NotLooking
            };
        }

        /// <summary>Создаёт команду создания мероприятия с валидными данными (дата в будущем).</summary>
        public static CreateEventCommand CreateValidEventCommand(string title, DateTime? startDateTime = null)
        {
            return new CreateEventCommand
            {
                Title = title,
                RegionId = 1,
                CityId = 1,
                Address = "Test Address",
                StartDateTime = startDateTime ?? DateTime.UtcNow.AddDays(7),
                MaxParticipants = 10
            };
        }

        /// <summary>Создаёт команду отправки предложения о сотрудничестве.</summary>
        public static SendSuggestionCommand CreateSendSuggestionCommand(Guid toProfileId, string? message = null)
        {
            return new SendSuggestionCommand
            {
                ToProfileId = toProfileId,
                Message = message ?? "Test collaboration message"
            };
        }

        /// <summary>Создаёт запрос поиска профилей с пагинацией по умолчанию.</summary>
        public static SearchProfilesQuery CreateDefaultSearchQuery() => new SearchProfilesQuery { Page = 1, Limit = 20 };

        /// <summary>Создаёт запрос получения мероприятий с пагинацией по умолчанию.</summary>
        public static GetEventsQuery CreateDefaultEventsQuery() => new GetEventsQuery { Page = 1, Limit = 20 };
    }
}