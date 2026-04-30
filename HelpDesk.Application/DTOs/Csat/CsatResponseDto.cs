namespace HelpDesk.Application.DTOs.Csat
{
    public class CsatResponseDto
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string? Comments { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
