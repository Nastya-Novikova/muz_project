namespace MusicianFinder.API.Contracts.Responses
{
    public class ConnectVkRequest
    {
        public string Code { get; set; }
        public string CodeVerifier { get; set; }
        public string DeviceId { get; set; }
    }
}
