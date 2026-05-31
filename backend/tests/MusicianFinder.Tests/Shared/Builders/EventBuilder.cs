using System;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Tests.Shared.Builders
{
    /// <summary>
    /// Строитель для создания тестовых экземпляров <see cref="Event"/>.
    /// </summary>
    public class EventBuilder
    {
        private EventTitle _title = new EventTitle("Test Event");
        private string? _description = "Test description";
        private int _regionId = 1;
        private int _cityId = 1;
        private string _address = "Test Address";
        private DateTime _startDateTime = DateTime.UtcNow.AddDays(7);
        private DateTime? _endDateTime = null;
        private int _maxParticipants = 10;
        private Guid _creatorProfileId = Guid.NewGuid();

        /// <summary>Устанавливает название мероприятия.</summary>
        public EventBuilder WithTitle(string title) { _title = new EventTitle(title); return this; }

        /// <summary>Устанавливает описание мероприятия.</summary>
        public EventBuilder WithDescription(string? description) { _description = description; return this; }

        /// <summary>Устанавливает идентификатор региона.</summary>
        public EventBuilder WithRegionId(int regionId) { _regionId = regionId; return this; }

        /// <summary>Устанавливает идентификатор города.</summary>
        public EventBuilder WithCityId(int cityId) { _cityId = cityId; return this; }

        /// <summary>Устанавливает адрес проведения.</summary>
        public EventBuilder WithAddress(string address) { _address = address; return this; }

        /// <summary>Устанавливает дату и время начала.</summary>
        public EventBuilder WithStartDateTime(DateTime startDateTime) { _startDateTime = startDateTime; return this; }

        /// <summary>Устанавливает дату и время окончания.</summary>
        public EventBuilder WithEndDateTime(DateTime? endDateTime) { _endDateTime = endDateTime; return this; }

        /// <summary>Устанавливает максимальное количество участников.</summary>
        public EventBuilder WithMaxParticipants(int maxParticipants) { _maxParticipants = maxParticipants; return this; }

        /// <summary>Устанавливает идентификатор профиля создателя.</summary>
        public EventBuilder WithCreatorProfileId(Guid creatorProfileId) { _creatorProfileId = creatorProfileId; return this; }

        /// <summary>Создаёт экземпляр <see cref="Event"/>.</summary>
        public Event Build() => new Event(
            _title, _regionId, _cityId, _address, _startDateTime, _creatorProfileId,
            _description, _endDateTime, _maxParticipants);
    }
}