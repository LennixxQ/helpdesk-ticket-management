namespace HelpDesk.Application.DTOs.Dashboard
{
    public class TopAgentDto
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int ResolvedCount { get; set; }
    }
}
