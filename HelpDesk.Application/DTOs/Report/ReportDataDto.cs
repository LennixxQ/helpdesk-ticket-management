namespace HelpDesk.Application.DTOs.Report
{
    public class ReportDataDto
    {
        public int TotalTickets { get; set; }
        public Dictionary<string, int> ByStatus { get; set; } = new();
        public Dictionary<string, int> ByPriority { get; set; } = new();
        public Dictionary<string, int> ByCategory { get; set; } = new();
        public Dictionary<string, int> ByDay { get; set; } = new();
    }
}
