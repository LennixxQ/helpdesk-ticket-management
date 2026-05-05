namespace HelpDesk.Application.DTOs.Report
{
    public class SlaComplianceReportDto
    {
        public int TotalTickets { get; set; }
        public int WithinSla { get; set; }
        public int Breached { get; set; }
        public double CompliancePct { get; set; }
        public Dictionary<string, int> BreachedByPriority { get; set; } = new();
    }
}
