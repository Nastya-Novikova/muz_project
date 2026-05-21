using System;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Tests.Shared.Builders
{
    /// <summary>
    /// Строитель для создания тестовых экземпляров <see cref="CollaborationSuggestion"/>.
    /// </summary>
    public class CollaborationSuggestionBuilder
    {
        private Guid _fromProfileId = Guid.NewGuid();
        private Guid _toProfileId = Guid.NewGuid();
        private string? _message = null;

        public CollaborationSuggestionBuilder FromProfile(Guid fromId) { _fromProfileId = fromId; return this; }
        public CollaborationSuggestionBuilder ToProfile(Guid toId) { _toProfileId = toId; return this; }
        public CollaborationSuggestionBuilder WithMessage(string? message) { _message = message; return this; }

        /// <summary>Создаёт экземпляр <see cref="CollaborationSuggestion"/>.</summary>
        public CollaborationSuggestion Build() => new CollaborationSuggestion(_fromProfileId, _toProfileId, _message);
    }
}