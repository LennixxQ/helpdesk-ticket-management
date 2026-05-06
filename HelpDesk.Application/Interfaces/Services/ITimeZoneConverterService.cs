namespace HelpDesk.Application.Interfaces.Services
{
    public interface ITimeZoneConverterService
    {
        DateTime ConvertToLocal(DateTime utcDateTime);
        string GetTimeZoneAbbreviation();
    }
}
