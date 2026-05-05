namespace HelpDesk.Application.DTOs.MFA
{
    public class MfaSetupDto
    {
        public string SecretKey { get; set; } = string.Empty;
        public string QrCodeDataUri { get; set; } = string.Empty;
    }
}
