namespace backend.Services.Utils
{
    public static class NotificationMessageProvider
    {
        public static (string Title, string Message) GetCollaborationReceived(string fromProfileName)
        {
            return (
                $"Пользователь {fromProfileName} отправил вам предложение о сотрудничестве",
                "У вас новое предложение о сотрудничестве"
            );
        }

        public static (string Title, string Message) GetEventRegistration(string registeredProfileName, string eventTitle)
        {
            return (
                $"{registeredProfileName} записался на ваше мероприятие \"{eventTitle}\"",
                "Новый участник зарегистрировался на ваше мероприятие"
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
