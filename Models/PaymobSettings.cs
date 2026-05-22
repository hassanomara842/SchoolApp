namespace SchoolApp.Models
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string HMACSecret { get; set; } = string.Empty;
        public string IntegrationId { get; set; } = string.Empty;
        public string IframeId { get; set; } = string.Empty;
    }
}
