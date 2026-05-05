namespace HelpDesk.Application.DTOs.MFA
{
    public class VerifyMfaRequest
    {
        public string Code { get; set; } = string.Empty;
        public string? JwtToken { get; set; } // Used during login
    }
}
