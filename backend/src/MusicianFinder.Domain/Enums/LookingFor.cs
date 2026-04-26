namespace MusicianFinder.Domain.Enums
{
    /// <summary>
    /// Кого ищет пользователь.
    /// </summary>
    public enum LookingFor
    {
        /// <summary>
        /// Не ищет.
        /// </summary>
        NotLooking,

        /// <summary>
        /// Ищет музыканта.
        /// </summary>
        LookingForMusician,

        /// <summary>
        /// Ищет группу.
        /// </summary>
        LookingForBand
    }
}