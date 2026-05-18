namespace PerfectohubRu.Model
{
    public class IntegrationData
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsHtml { get; set; } = true;

        public bool HasIntegration =>
            !string.IsNullOrEmpty(Url) &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password)
            ;
    }
}
