using HelpDesk.Application.DTOs.Report;
using HelpDesk.Domain.Enums;

namespace HelpDesk.API.Records
{
    public record GetByIdRequest(Guid Id);
    public record GetByKeyRequest(string Key);
    public record AssignHeadRequest(Guid DepartmentId, Guid UserId);
    public record AgentStatsRequest(Guid AgentId, DateTime From, DateTime To);
    public record FeedbackRequest(Guid ArticleId, bool IsHelpful);
    public record UpdateSettingRequest(string Key, string Value);
    public record EscalateRequest(Guid TicketId, string Reason);
    public record AuditFilterRequest(DateTime? From, DateTime? To, string? Actor, string? Action, string? EntityType, int Page = 1, int PageSize = 20);
    public record AgentReportRequest(Guid? AgentId, ReportFilterDto Filter);
    public record AuditExportRequest(DateTime From, DateTime To);
    public record UpsertPreferenceRequest(NotificationEventType EventType, bool IsEnabled);
    public record ResolvedViaKbRequest(Guid TicketId, Guid ArticleId);
    public record MoveDepartmentRequest(Guid UserId, Guid DepartmentId);
    public record TestEmailRequest(string ToEmail);
    public record CsatSubmission(Guid TicketId, int Rating, string? Comments);
}
