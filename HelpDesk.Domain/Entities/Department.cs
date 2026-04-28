using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? DepartmentHeadId { get; set; }


        public User? DepartmentHead { get; set; }
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
