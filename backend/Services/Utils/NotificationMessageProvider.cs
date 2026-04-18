namespace backend.Services.Utils
{
    public static class NotificationMessageProvider
    {
        public static (string Title, string Message) GetCollaborationReceived(string fromProfileName, string? suggestionMessage)
        {
            var body = string.IsNullOrWhiteSpace(suggestionMessage)
                ? "У вас новое предложение о сотрудничестве"
                : $"Сообщение: {suggestionMessage}";

            return (
                $"Пользователь {fromProfileName} отправил вам предложение о сотрудничестве",
                body
            );
        }

        public static (string Title, string Message) GetEventRegistration(string eventTitle)
        {
            return (
                "Регистрация подтверждена",
                $"Вы успешно зарегистрировались на мероприятие \"{eventTitle}\""
            );
        }

        public static (string Title, string Message) GetEventReminder(string eventTitle, int daysLeft)
        {
            return (
                $"Через {daysLeft} дн. состоится мероприятие \"{eventTitle}\"",
                "Не забудьте о предстоящем мероприятии"
            );
        }
    }
}
