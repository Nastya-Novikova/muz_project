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
                    data.TryGetValue("message", out var msg) && msg != null
                        ? $"Сообщение: {msg}"
                        : "У вас новое предложение"
                ),
                NotificationType.EventRegistration => (
                    "Регистрация подтверждена",
                    BuildEventRegistrationMessage(data)
                ),
                NotificationType.EventReminder => (
                    $"Через {data["daysLeft"]} дн. состоится мероприятие \"{data["eventTitle"]}\"",
                    "Не забудьте о предстоящем мероприятии"
                ),
                _ => ("Новое уведомление", string.Empty)
            };
        }

        private static string BuildEventRegistrationMessage(IReadOnlyDictionary<string, object> data)
        {
            var title = data.TryGetValue("eventTitle", out var t) ? t?.ToString() : "Мероприятие";
            var address = data.TryGetValue("address", out var a) ? a?.ToString() : "";
            var city = data.TryGetValue("cityName", out var c) ? c?.ToString() : "";
            var region = data.TryGetValue("regionName", out var r) ? r?.ToString() : "";
            var startDateTime = data.TryGetValue("startDateTime", out var sdt) ? sdt : null;

            var locationParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(address)) locationParts.Add(address);
            if (!string.IsNullOrWhiteSpace(city)) locationParts.Add(city);
            if (!string.IsNullOrWhiteSpace(region)) locationParts.Add(region);
            var location = locationParts.Count > 0 ? string.Join(", ", locationParts) : null;

            var datePart = startDateTime is DateTime dt ? $" {dt:dd.MM.yyyy HH:mm}" : "";

            return $"Вы успешно зарегистрировались на мероприятие \"{title}\".{datePart}." +
                   (location != null ? $" Оно будет проходить по адресу: {location}." : "");
        }
    }
}