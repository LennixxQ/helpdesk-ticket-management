namespace HelpDesk.Application.DTOs.Report
{
    public class AgentSelfServiceReportDto
    {
        public int ResolvedThisMonth { get; set; }
        public double AvgResolutionHours { get; set; }
        public double SlaCompliancePct { get; set; }
        public double? CsatScore { get; set; }
        public int TotalComments { get; set; }
    }
}
