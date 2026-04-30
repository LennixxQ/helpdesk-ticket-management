using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class RecurringTemplate : BaseEntity
{
    public string TemplateName { get; set; } = string.Empty;
    public string TicketTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public RecurrencePattern RecurrencePattern { get; set; }
    public string? CronExpression { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxOccurrences { get; set; }
    public int RunCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? NextRunAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CreatedByAdminId { get; set; }
    public Guid? AssignToAgentId { get; set; }
    public Guid RaiseOnBehalfOfId { get; set; }

    public Category Category { get; set; } = null!;
    public User CreatedByAdmin { get; set; } = null!;
    public User? AssignToAgent { get; set; }
    public User RaiseOnBehalfOf { get; set; } = null!;
    public ICollection<RecurringTemplateRun> Runs { get; set; } = new List<RecurringTemplateRun>();

}