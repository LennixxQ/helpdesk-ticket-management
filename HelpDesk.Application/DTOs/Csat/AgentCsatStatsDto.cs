namespace HelpDesk.Application.DTOs.Csat
{
    public class AgentCsatStatsDto
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public double? AverageScore { get; set; }
        public int ResponseCount { get; set; }
    }
}
