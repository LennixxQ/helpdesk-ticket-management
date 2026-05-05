namespace HelpDesk.Application.DTOs.Report
{
    public record ReportFilterDto
    {
        public DateTime From { get; set; } = DateTime.UtcNow.AddMonths(-1);
        public DateTime To { get; set; } = DateTime.UtcNow;
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
