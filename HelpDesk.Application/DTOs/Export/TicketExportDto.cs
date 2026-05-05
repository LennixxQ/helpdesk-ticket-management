namespace HelpDesk.Application.DTOs.Export
{
    public class TicketExportDto
    {
        public string TicketId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string RaisedBy { get; set; } = string.Empty;
        public string AssignedAgent { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string SlaDeadline { get; set; } = string.Empty;
        public string SlaStatus { get; set; } = string.Empty;
        public string IsEscalated { get; set; } = string.Empty;
    }
}
