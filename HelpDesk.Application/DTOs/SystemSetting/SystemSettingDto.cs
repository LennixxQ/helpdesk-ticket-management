namespace HelpDesk.Application.DTOs.SystemSetting
{
    public class SystemSettingDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
