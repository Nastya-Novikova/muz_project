using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Контекст базы данных только для чтения, используемый в обработчиках запросов.
    /// </summary>
    public interface IReadDbContext
    {
        /// <summary>
        /// Запрос к профилям музыкантов.
        /// </summary>
        IQueryable<MusicianProfile> Profiles { get; }

        /// <summary>
        /// Запрос к мероприятиям.
        /// </summary>
        IQueryable<Event> Events { get; }

        /// <summary>
        /// Запрос к предложениям о сотрудничестве.
        /// </summary>
        IQueryable<CollaborationSuggestion> CollaborationSuggestions { get; }

        /// <summary>
        /// Запрос к уведомлениям.
        /// </summary>
        IQueryable<Notification> Notifications { get; }

        /// <summary>
        /// Запрос к городам.
        /// </summary>
        IQueryable<City> Cities { get; }

        /// <summary>
        /// Запрос к регионам.
        /// </summary>
        IQueryable<Region> Regions { get; }

        /// <summary>
        /// Запрос к жанрам.
        /// </summary>
        IQueryable<Genre> Genres { get; }

        /// <summary>
        /// Запрос к музыкальным специальностям.
        /// </summary>
        IQueryable<MusicalSpecialty> Specialties { get; }

        /// <summary>
        /// Запрос к целям сотрудничества.
        /// </summary>
        IQueryable<CollaborationGoal> CollaborationGoals { get; }

        /// <summary>
        /// Запрос к пользователям.
        /// </summary>
        IQueryable<User> Users { get; }

        /// <summary>
        /// Запрос к кодам подтверждения email.
        /// </summary>
        IQueryable<EmailVerificationCode> EmailVerificationCodes { get; }
    }
}