namespace HelpDesk.Domain.Entities
{
    public class DepartmentSummary
    {
        public int ActiveUserCount { get; set; }
        public int OpenTicketCount { get; set; }
        public int TicketsLast30Days { get; set; }
    }
}
