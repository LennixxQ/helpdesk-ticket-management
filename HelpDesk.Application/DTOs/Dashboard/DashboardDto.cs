namespace HelpDesk.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalTickets { get; set; }
        public Dictionary<string, int> TicketsByStatus { get; set; } = new();
        public Dictionary<string, int> TicketsByPriority { get; set; } = new();
        public int TicketsThisMonth { get; set; }
        public int TicketsLastMonth { get; set; }
        public List<TopAgentDto> TopAgentsThisMonth { get; set; } = new();
    }
}
