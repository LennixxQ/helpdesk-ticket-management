namespace HelpDesk.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalTickets { get; set; }
        public Dictionary<string, int> TicketsByStatus { get; set; } = new();
        public Dictionary<string, int> TicketsByPriority { get; set; } = new();
        public List<TopAgentDto> TopAgentsThisMonth { get; set; } = new();
        public int TicketsThisMonth { get; set; }
        public int TicketsLastMonth { get; set; }
    }
}
