namespace HelpDesk.Domain.Entities;

public class RecurringTemplateRun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public Guid? GeneratedTicketId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime ActualRunAt { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }

    public RecurringTemplate Template { get; set; } = null!;
    public Ticket? GeneratedTicket { get; set; }
}