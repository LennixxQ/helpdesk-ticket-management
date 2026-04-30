using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.RecurringTemplate
{
    public class RecurringTemplateDto
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string TicketTitle { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public RecurrencePattern RecurrencePattern { get; set; }
        public bool IsActive { get; set; }
        public DateTime? NextRunAt { get; set; }
        public DateTime? LastRunAt { get; set; }
        public int RunCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
