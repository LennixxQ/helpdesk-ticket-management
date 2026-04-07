namespace HelpDesk.Application.DTOs;

public class CreateTicketResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}