using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Commands.RecurringTemplateCommand
{
    public class CreateRecurringTemplateCommand
    {
        public string TemplateName { get; set; } = string.Empty;
        public string TicketTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public Guid CategoryId { get; set; }
        public RecurrencePattern RecurrencePattern { get; set; }
        public string? CronExpression { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxOccurrences { get; set; }
        public Guid? AssignToAgentId { get; set; }
        public Guid RaiseOnBehalfOfId { get; set; }
    }
}
