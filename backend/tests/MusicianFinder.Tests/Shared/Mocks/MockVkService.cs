using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Tests.Shared.Mocks
{
    public class MockVkService : IVkService
    {
        private long? _exchangedUserId = 123456789L; // значение по умолчанию

        public string? LastExchangeCode { get; private set; }
        public Guid? LastSentUserId { get; private set; }
        public string? LastSentMessage { get; private set; }

        /// <summary>
        /// Установить ID пользователя VK, который будет возвращён ExchangeCodeAsync.
        /// Передайте null, чтобы имитировать ошибку обмена.
        /// </summary>
        public void SetExchangedUserId(long? userId) => _exchangedUserId = userId;

        public Task ConnectVkAsync(Guid profileId, string code, string codeVerifier, string deviceId)
        {
            return Task.CompletedTask;
        }

        public Task<long?> ExchangeCodeAsync(string code, string codeVerifier, string deviceId)
        {
            LastExchangeCode = code;
            return Task.FromResult(_exchangedUserId);
        }

        public Task<bool> SendNotificationAsync(Guid userId, string message)
        {
            LastSentUserId = userId;
            LastSentMessage = message;
            return Task.FromResult(true);
        }
    }
}