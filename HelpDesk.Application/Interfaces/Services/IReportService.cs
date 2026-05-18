using HelpDesk.Application.DTOs.Report;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<byte[]> ExportAuditLogCsvAsync(DateTime from, DateTime to);
        Task<byte[]> ExportTicketsCsvAsync(ReportFilterDto filter);
        Task<byte[]> ExportTicketsPdfAsync(ReportFilterDto filter);
        Task<ReportDataDto> GetTicketVolumeReportAsync(ReportFilterDto filter);
        Task<AgentPerformanceReportDto> GetAgentPerformanceReportAsync(Guid? agentId, ReportFilterDto filter);
        Task<SlaComplianceReportDto> GetSlaComplianceReportAsync(ReportFilterDto filter);
        Task<AgentSelfServiceReportDto> GetAgentSelfServiceReportAsync(Guid agentId);
        Task<AgentPerformanceReportDto> GetTopAgentReportAsync(ReportFilterDto filter);
    }
}
