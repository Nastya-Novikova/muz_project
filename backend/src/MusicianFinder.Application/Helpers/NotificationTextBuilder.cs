using System.Collections.Generic;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Helpers
{
    /// <summary>
    /// Предоставляет статические методы для формирования текстов уведомлений на основе типа и переданных данных.
    /// </summary>
    public static class NotificationTextBuilder
    {
        /// <summary>
        /// Возвращает заголовок и сообщение для уведомления заданного типа.
        /// </summary>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Словарь с данными (имена параметров зависят от типа).</param>
        /// <returns>Кортеж (заголовок, сообщение).</returns>
        public static (string Title, string Message) Build(NotificationType type, IReadOnlyDictionary<string, object> data)
        {
            return type switch
            {
                NotificationType.CollaborationReceived => (
                    $"Пользователь {data["fromProfileName"]} отправил вам предложение о сотрудничестве",
                    data.TryGetValue("message", out var msg) ? msg?.ToString() ?? "У вас новое предложение" : "У вас новое предложение"
                ),
                NotificationType.EventRegistration => (
                    "Регистрация подтверждена",
                    $"Вы успешно зарегистрировались на мероприятие \"{data["eventTitle"]}\""
                ),
                NotificationType.EventReminder => (
                    $"Через {data["daysLeft"]} дн. состоится мероприятие \"{data["eventTitle"]}\"",
                    "Не забудьте о предстоящем мероприятии"
                ),
                _ => ("Новое уведомление", string.Empty)
            };
        }
    }
}