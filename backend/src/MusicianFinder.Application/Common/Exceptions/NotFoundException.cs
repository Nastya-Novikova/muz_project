using System;

namespace MusicianFinder.Application.Common.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о том, что запрашиваемый ресурс не найден.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotFoundException"/>.
        /// </summary>
        public NotFoundException() : base("Запрашиваемый ресурс не найден.") { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotFoundException"/> с указанным сообщением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotFoundException"/> для конкретной сущности.
        /// </summary>
        /// <param name="name">Название сущности.</param>
        /// <param name="key">Ключ сущности.</param>
        public NotFoundException(string name, object key) : base($"Сущность \"{name}\" с ключом ({key}) не найдена.") { }
    }
}