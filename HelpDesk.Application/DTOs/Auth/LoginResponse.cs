namespace HelpDesk.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? MfaSessionToken { get; set; }
        public bool RequiresSetup { get; set; }
    }
}
