using System.Net.Sockets;

namespace HelpDesk.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Ticket> Tickets { get; set; } = [];
}