namespace HelpDesk.Application.Interfaces.Services
{
    public interface ISlaDeadlineCalculator
    {
        DateTime Calculate(DateTime from, int resolutionMinutes);
    }
}
