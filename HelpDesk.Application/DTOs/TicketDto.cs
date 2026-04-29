using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsEscalated { get; set; }
        public SlaStatus SlaStatus { get; set; }
        public DateTime? SlaDeadline { get; set; }
        public bool SlaBreached { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string RaisedByUserName { get; set; } = string.Empty;
        public Guid RaisedByUserId { get; set; }
        public string? AssignedAgentName { get; set; }
        public Guid? AssignedAgentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }
}
