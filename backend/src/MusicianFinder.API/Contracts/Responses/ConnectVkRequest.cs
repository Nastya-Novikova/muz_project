namespace MusicianFinder.API.Contracts.Responses
{
    public class ConnectVkRequest
    {
        /// <summary>
        /// Код авторизации OAuth, полученный от VK.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// PKCE-верификатор кода, созданный на клиенте.
        /// </summary>
        public string CodeVerifier { get; set; }
        /// <summary>
        /// Уникальный идентификатор устройства для VK.
        /// </summary>
        public string DeviceId { get; set; }
    }
}
