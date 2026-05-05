namespace HelpDesk.Application.DTOs.Report
{
    public class AgentPerformanceReportDto
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int TotalResolved { get; set; }
        public double AvgResolutionHours { get; set; }
        public double SlaCompliancePct { get; set; }
        public double? CsatScore { get; set; }
    }
}
