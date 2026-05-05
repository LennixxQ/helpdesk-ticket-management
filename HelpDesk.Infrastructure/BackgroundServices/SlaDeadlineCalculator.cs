using HelpDesk.Application.Interfaces.Services;

namespace HelpDesk.Infrastructure.BackgroundServices
{
    public class SlaDeadlineCalculator : ISlaDeadlineCalculator
    {
        private readonly IBusinessHoursService _bhs;

        public SlaDeadlineCalculator(IBusinessHoursService bhs) => _bhs = bhs;

        public DateTime Calculate(DateTime from, int resolutionMinutes) => _bhs.AddBusinessMinutes(from, resolutionMinutes);
    }
}
