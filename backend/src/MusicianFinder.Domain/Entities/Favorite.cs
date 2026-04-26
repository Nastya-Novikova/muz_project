namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Запись в избранном: связь между двумя музыкальными профилями.
    /// Принадлежит агрегату MusicianProfile того, кто добавил в избранное.
    /// </summary>
    public class Favorite
    {
        private Favorite() { }

        /// <summary>
        /// Инициализирует новую запись избранного.
        /// </summary>
        /// <param name="addedByProfileId">Идентификатор профиля, который добавил в избранное.</param>
        /// <param name="targetProfileId">Идентификатор профиля, добавленного в избранное.</param>
        public Favorite(Guid addedByProfileId, Guid targetProfileId)
        {
            AddedByProfileId = addedByProfileId;
            TargetProfileId = targetProfileId;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор профиля, добавившего в избранное.
        /// </summary>
        public Guid AddedByProfileId { get; private set; }

        /// <summary>
        /// Идентификатор профиля, добавленного в избранное.
        /// </summary>
        public Guid TargetProfileId { get; private set; }

        /// <summary>
        /// Дата добавления.
        /// </summary>
        public DateTime CreatedAt { get; private set; }
    }
}