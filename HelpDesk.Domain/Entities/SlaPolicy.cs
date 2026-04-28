using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class SlaPolicy : BaseEntity
{
    public TicketPriority Priority { get; set; }
    public int FirstResponseMinutes { get; set; }
    public int ResolutionMinutes { get; set; }
    public bool IsActive { get; set; } = true;
}