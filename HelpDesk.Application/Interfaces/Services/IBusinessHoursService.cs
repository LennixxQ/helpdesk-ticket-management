namespace HelpDesk.Application.Interfaces.Services
{
    public interface IBusinessHoursService
    {
        bool IsBusinessHour(DateTime utcDateTime);
        bool IsWorkingDay(DateOnly date);
        DateTime GetNextBusinessStart(DateTime utcDateTime);
        DateTime AddBusinessMinutes(DateTime utcStart, int minutes);
    }
}
