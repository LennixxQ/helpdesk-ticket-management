namespace HelpDesk.Application.DTOs
{
    public class TopAgentDto
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int ResolvedCount { get; set; }
    }
}
