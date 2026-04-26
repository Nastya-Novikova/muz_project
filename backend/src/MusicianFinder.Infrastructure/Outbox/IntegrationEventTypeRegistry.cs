using System.Collections.Concurrent;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Outbox
{
    /// <summary>
    /// Реестр типов интеграционных событий, сопоставляющий строковое имя события с CLR-типом.
    /// </summary>
    public class IntegrationEventTypeRegistry : IIntegrationEventTypeRegistry
    {
        private readonly ConcurrentDictionary<string, Type> _types = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IntegrationEventTypeRegistry"/> и регистрирует все известные события.
        /// </summary>
        public IntegrationEventTypeRegistry()
        {
            Register<ProfileCreatedIntegrationEvent>("profile.created", 1);
            Register<ProfileCoreInfoUpdatedIntegrationEvent>("profile.core_info_updated", 1);
            Register<ProfileContactsUpdatedIntegrationEvent>("profile.contacts_updated", 1);
            Register<ProfileGenresChangedIntegrationEvent>("profile.genres_changed", 1);
            Register<ProfileSpecialtiesChangedIntegrationEvent>("profile.specialties_changed", 1);
            Register<ProfileCollaborationGoalsChangedIntegrationEvent>("profile.collaboration_goals_changed", 1);
            Register<ProfileDesiredGenresChangedIntegrationEvent>("profile.desired_genres_changed", 1);
            Register<ProfileDesiredSpecialtiesChangedIntegrationEvent>("profile.desired_specialties_changed", 1);
            Register<ProfileDeletedIntegrationEvent>("profile.deleted", 1);
            Register<PortfolioItemAddedIntegrationEvent>("portfolio.item_added", 1);
            Register<PortfolioItemRemovedIntegrationEvent>("portfolio.item_removed", 1);
            Register<FavoriteAddedIntegrationEvent>("favorite.added", 1);
            Register<FavoriteRemovedIntegrationEvent>("favorite.removed", 1);
            Register<EventCreatedIntegrationEvent>("event.created", 1);
            Register<EventUpdatedIntegrationEvent>("event.updated", 1);
            Register<EventCancelledIntegrationEvent>("event.cancelled", 1);
            Register<UserRegisteredToEventIntegrationEvent>("event.user_registered", 1);
            Register<UserUnregisteredFromEventIntegrationEvent>("event.user_unregistered", 1);
            Register<CollaborationSuggestionSentIntegrationEvent>("collaboration.suggestion_sent", 1);
            Register<CollaborationSuggestionAcceptedIntegrationEvent>("collaboration.suggestion_accepted", 1);
            Register<CollaborationSuggestionRejectedIntegrationEvent>("collaboration.suggestion_rejected", 1);
        }

        /// <inheritdoc />
        public Type Resolve(string eventName, int version)
        {
            var key = $"{eventName}:{version}";
            if (!_types.TryGetValue(key, out var type))
                throw new InvalidOperationException($"Неизвестное событие: {key}");

            return type;
        }

        private void Register<T>(string eventName, int version) where T : IIntegrationEvent
        {
            _types[$"{eventName}:{version}"] = typeof(T);
        }
    }
}