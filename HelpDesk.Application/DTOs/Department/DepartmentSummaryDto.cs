namespace HelpDesk.Application.DTOs.Department
{
    public class DepartmentSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ActiveUserCount { get; set; }
        public int OpenTicketCount { get; set; }
        public int TicketsLast30Days { get; set; }
    }
}
