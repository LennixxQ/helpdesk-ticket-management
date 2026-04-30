using HelpDesk.Application.DTOs.Comment;
using HelpDesk.Application.DTOs.User;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public bool IsEscalated { get; set; }
        public SlaStatus SlaStatus { get; set; }
        public DateTime? SlaDeadline { get; set; }
        public bool SlaBreached { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public UserDto? RaisedByUser { get; set; }
        public UserDto? AssignedAgent { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
        public EscalationDto? Escalation { get; set; }
    }
}
